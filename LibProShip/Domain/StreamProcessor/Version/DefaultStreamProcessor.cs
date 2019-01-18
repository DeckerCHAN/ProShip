using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibProShip.Domain.StreamProcessor.Packet;
using LibProShip.Infrastructure.Unpickling;

namespace LibProShip.Domain.StreamProcessor.Version
{
    public sealed class DefaultStreamProcessor : IStreamProcessor
    {
        public BattleRecord ProcessStream(byte[] data)
        {
            var p = new InnerProcessor(data);
            return p.GetRecord();
        }
    }


    internal class InnerProcessor
    {
        private static readonly IReadOnlyDictionary<int, double> _mapIdConvertRatio = new Dictionary<int, double>
        {
            {34, 24000D / 1600D},
            {33, 24000D / 1600D},
            {01, 30000D / 1600D},
            {10, 30000D / 1600D},
            {04, 30000D / 1600D},
            {05, 36000D / 1600D},
            {08, 36000D / 1600D},
            {13, 36000D / 1600D},
            {17, 42000D / 1600D},
            {41, 42000D / 1600D},
            {15, 48000D / 1600D},
            {35, 48000D / 1600D},
            {46, 42000D / 1600D},
            {23, 42000D / 1600D},
            {42, 42000D / 1600D},
            {50, 42000D / 1600D},
            {20, 42000D / 1600D},
            {16, 48000D / 1600D},
            {22, 48000D / 1600D},
            {19, 42000D / 1600D},
            {28, 42000D / 1600D},
            {40, 42000D / 1600D},
            {18, 48000D / 1600D},
            {14, 42000D / 1600D},
            {38, 48000D / 1600D},
            {37, 48000D / 1600D},
            {44, 48000D / 1600D},
            {25, 48000D / 1600D},
            {45, 48000D / 1600D},
            {00, 36000D / 1600D}
        };

        private readonly byte[] Data;


        public InnerProcessor(byte[] data)
        {
            Data = data;
            ShipEntityIds = new List<int>();
            Alies = new List<Vehicle>();
            Enemy = new List<Vehicle>();
            EntityIdPlayer = new Dictionary<int, Player>();
            EntityIdVehicle = new Dictionary<int, Vehicle>();
            PositionRecords = new List<PositionRecord>();
            DamageRecords = new List<DamageRecord>();
            GunShootRecords = new List<GunShootRecord>();
            TorpedoShootRecords = new List<TorpedoShootRecord>();
            HitRecords = new List<HitRecord>();
        }

        private ICollection<Vehicle> Enemy { get; }
        private ICollection<Vehicle> Alies { get; }

        private ICollection<Vehicle> Vehicles => Enemy.Concat(Alies).ToList();

        private IList<PositionRecord> PositionRecords { get; }
        public IList<DamageRecord> DamageRecords { get; set; }
        private IList<GunShootRecord> GunShootRecords { get; }
        private IList<HitRecord> HitRecords { get; }
        private IList<TorpedoShootRecord> TorpedoShootRecords { get; }
        private BattleRecord ProcessResult { get; set; }
        private IDictionary<int, Player> EntityIdPlayer { get; }
        private IDictionary<int, Vehicle> EntityIdVehicle { get; }
        private int AvatarId { get; set; }
        private List<int> ShipEntityIds { get; }
        public Map Map { get; private set; }

        private Vehicle ControlVehicle { get; set; }

        public long ArenaId { get; set; }

        public BattleRecord GetRecord()
        {
            using (var st = new MemoryStream(Data))
            using (var binaryReader = new BinaryReader(st))
            {
                while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                {
                    var size = binaryReader.ReadUInt32();
                    var type = binaryReader.ReadUInt32();
                    var time = binaryReader.ReadSingle();

                    using (var chunk = new MemoryStream(binaryReader.ReadBytes((int) size)))
                    using (var reader = new BinaryReader(chunk))
                    {
                        switch (type)
                        {
                            case 0:
                                BasePlayerCrate(reader);
                                break;
                            case 0x5:
                                Entity(reader);
                                break;
                            case 0x8:
                                EntityMethod(time, reader);
                                break;
                            case 1:
                                CellPlayerCreate(reader);
                                break;
                            case 10:
                                DecodeOtherPlayerPosition(time, reader);
                                break;
                            case 43:
                                DecodeControlPlayerPosition(time, reader);
                                break;
                            case 0x27:
                                DecodeMap(reader);
                                break;
                        }
                    }
                }
            }


            if (ProcessResult == null)
                ProcessResult = new BattleRecord(ArenaId, Map, ControlVehicle,
                    EntityIdPlayer.Values,
                    Enemy,
                    Alies,
                    PositionRecords.Distinct(),
                    TorpedoShootRecords.Distinct(),
                    GunShootRecords.Distinct(),
                    HitRecords.Distinct(),
                    DamageRecords.Distinct());

            return ProcessResult;
        }

        private void DecodeOtherPlayerPosition(float time, BinaryReader reader)
        {
            var entityId = Convert.ToInt32(reader.ReadUInt32());

            if (reader.BaseStream.Length > 45)
            {
                var spaceId = Convert.ToInt32(reader.ReadUInt32());
            }

            var vehicleId = Convert.ToInt32(reader.ReadUInt32());
            var position = Read3D(reader);
            var positionError = Read3D(reader);
            var rotation = Read3D(reader);
            var isErrored = reader.ReadBoolean();

            if (!EntityIdVehicle.ContainsKey(entityId)) return;

            var vehicle = EntityIdVehicle[entityId] ?? throw new Exception();
            PositionRecords.Add(new PositionRecord(time, vehicle, position, rotation));
        }

        private void EntityMethod(float time, BinaryReader reader)
        {
            var entityId = Convert.ToInt32(reader.ReadUInt32());
            var messageId = Convert.ToInt32(reader.ReadUInt32());

            if (entityId == AvatarId)
                switch (messageId)
                {
                    //Avatar 
                    case 84:
                        AddPlayer(reader);
                        break;
                    case 73:
                        AddGunAndTorpedo(time, reader);
                        break;
                }
            else if (Vehicles.Select(x => x.VehicleId).Contains(entityId))
                switch (messageId)
                {
                    case 51:
                        AddDamage(time, entityId, reader);
                        break;
                }
        }

        private void AddDamage(float time, int entityId, BinaryReader reader)
        {
            var dataLength = reader.ReadInt32();
            var damageLength = Convert.ToInt32(reader.ReadByte());

            for (var i = 0; i < damageLength; i++)
            {
                var vehicleId = reader.ReadInt32();
                var damageAmount = reader.ReadSingle();

                DamageRecords.Add(new DamageRecord(time, EntityIdVehicle[vehicleId],
                    EntityIdVehicle[entityId], damageAmount));
            }
        }

        private void AddGunAndTorpedo(float time, BinaryReader reader)
        {
            var dataLength = reader.ReadInt32();
            var shotLength = Convert.ToInt32(reader.ReadByte());
            for (var i = 0; i < shotLength; i++) AddGun(time, reader);

            var torpedoLength = Convert.ToInt32(reader.ReadByte());
            for (var i = 0; i < torpedoLength; i++) AddTorpedo(time, reader);

            var hitLength = Convert.ToInt32(reader.ReadByte());
            for (var i = 0; i < hitLength; i++) AddHit(time, reader);
        }

        private void AddHit(float time, BinaryReader reader)
        {
            var pos = Read3D(reader);
            var ownerId = reader.ReadInt32();
            var shotId = Convert.ToInt32(reader.ReadUInt16());
            var hitType = Convert.ToInt32(reader.ReadByte());

            var vehicle = Vehicles.FirstOrDefault(x => x.VehicleId == ownerId) ?? throw new Exception();

            if (Enum.IsDefined(typeof(HitType), hitType))
                HitRecords.Add(new HitRecord(vehicle, time, pos, shotId, (HitType) hitType));
            else
                throw new Exception($"Unknown hit type id {hitType}");
        }


        private void AddTorpedo(float time, BinaryReader reader)
        {
            var gameparamsId = reader.ReadUInt32();
            var pos = Read3D(reader);
            var dir = Read3D(reader);
            var ownerId = reader.ReadInt32();
            var salvoId = reader.ReadInt32();
            var shotId = Convert.ToInt32(reader.ReadUInt16());
            var skinId = reader.ReadUInt32();

            var vehicle = Vehicles.FirstOrDefault(x => x.VehicleId == ownerId) ?? throw new Exception();


            TorpedoShootRecords.Add(new TorpedoShootRecord(vehicle, time, shotId, salvoId, pos, dir));
        }

        private void AddGun(float time, BinaryReader reader)
        {
            var gameparamsId = reader.ReadUInt32();
            var pos = Read3D(reader);
            var dir = Read3D(reader);
            var tarPos = Read3D(reader);
            var ownerId = reader.ReadInt32();
            var salvoId = reader.ReadInt32();
            var shotId = Convert.ToInt32(reader.ReadUInt16());
            var gunBarrelId = Convert.ToInt32(reader.ReadByte());
            var serverTimeLeft = reader.ReadSingle();
            var shooterHeight = reader.ReadSingle();
            var hitDistance = reader.ReadSingle();

            var vehicle = Vehicles.FirstOrDefault(x => x.VehicleId == ownerId) ?? throw new Exception();


            GunShootRecords.Add(new GunShootRecord(vehicle, time, shotId, salvoId, pos, dir, tarPos,
                hitDistance, gunBarrelId));
        }

        private void AddPlayer(BinaryReader reader)
        {
            var length = reader.ReadInt32();

            var areadId = reader.ReadInt64();
            ArenaId = areadId;
            var teamBuiltId = Convert.ToInt32(reader.ReadByte());
            var playersStates = ReadBlob(reader);

            foreach (var playersState in playersStates)
            {
                var name = playersState[20][1];
                var id = playersState[0][1];
                var vehicleId = playersState[27][1];
                var shipId = playersState[29][1];
                var player = new Player(name, id);
                var team = playersState[30][1];
                var avatarId = playersState[1][1];
                EntityIdPlayer[avatarId] = player;

                var vehicle = new Vehicle(vehicleId, player, shipId);
                EntityIdVehicle[vehicleId] = vehicle;
                EntityIdVehicle[avatarId] = vehicle;

                if (avatarId == AvatarId) ControlVehicle = vehicle;

                switch (team)
                {
                    case 0:
                        Enemy.Add(vehicle);
                        break;
                    case 1:
                        Alies.Add(vehicle);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        private void Entity(BinaryReader reader)
        {
            var entityId = reader.ReadInt32();
            var entityType = reader.ReadInt16();
            var spaceId = reader.ReadUInt32();
            var vehicleId = reader.ReadInt32();
            var position = Read3D(reader);
            var rotation = Read3D(reader);

            switch (entityType)
            {
                case 2:
                    ShipEntityIds.Add(entityId);
                    VehicleEntityCreate(entityId, reader);


                    break;
            }
        }

        private void VehicleEntityCreate(int entityId, BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var count = Convert.ToInt32(reader.ReadByte());

            if (count != 37) throw new Exception();

            var curse = 0;


            var isAntiAirMode = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var burningFlag = reader.ReadByte() == curse++ ? reader.ReadByte() : throw new Exception();
            var buoyancyCurrentState = reader.ReadByte() == curse++ ? reader.ReadByte() : throw new Exception();
            var isOnForsage = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var teamId = reader.ReadByte() == curse++ ? reader.ReadByte() : throw new Exception();
            var uiEnabled = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var isAlive = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var speedSignDir = reader.ReadByte() == curse++ ? reader.ReadSByte() : throw new Exception();
            var enginePower = reader.ReadByte() == curse++ ? reader.ReadByte() : throw new Exception();
            var isInOfflineMode = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var ignoreMapBorders = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var isBot = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var isFogHornOn = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var blockedControls = reader.ReadByte() == curse++ ? reader.ReadBoolean() : throw new Exception();
            var targetLocalPos = reader.ReadByte() == curse++ ? reader.ReadUInt16() : throw new Exception();
            var torpedoLocalPos = reader.ReadByte() == curse++ ? reader.ReadUInt16() : throw new Exception();
            var weaponLockFlags = reader.ReadByte() == curse++ ? reader.ReadUInt16() : throw new Exception();
            var serverSpeedRaw = reader.ReadByte() == curse++ ? reader.ReadUInt16() : throw new Exception();
            var airDefenseDispRadius = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            var health = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            var regenerationHealth = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            var regeneratedHealth = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            var oxygen = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            var buoyancyCurrentWaterline = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            var regenCrewHpLimit = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            var buoyancy = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            var owner = reader.ReadByte() == curse++ ? reader.ReadInt32() : throw new Exception();
            var selectedWeapon = reader.ReadByte() == curse++ ? reader.ReadUInt32() : throw new Exception();

            var draught = reader.ReadByte() == curse++ ? reader.ReadSingle() : throw new Exception();
            float effectiveness = 0;
            ulong learnedSkills = 0;
            uint paramsId = 0;

            if (reader.ReadByte() == curse++)
            {
                effectiveness = reader.ReadSingle();
                learnedSkills = reader.ReadUInt64();
                paramsId = reader.ReadUInt32();
            }
            else
            {
                throw new Exception();
            }

            var atbaTargets = new uint [0];

            if (reader.ReadByte() == curse++)
            {
                atbaTargets = new uint[reader.ReadByte()];
                for (var i = 0; i < atbaTargets.Length; i++) atbaTargets[i] = reader.ReadUInt32();
            }
            else
            {
                throw new Exception();
            }

            var airDefenceTargetIds = new long[0];
            if (reader.ReadByte() == curse++)
            {
                airDefenceTargetIds = new long[reader.ReadByte()];
                for (var i = 0; i < airDefenceTargetIds.Length; i++) airDefenceTargetIds[i] = reader.ReadInt64();
            }
            else
            {
                throw new Exception();
            }

            var airDefenceAuraId = -1;
            var airDefenceAuraEnabled = false;

            if (reader.ReadByte() == curse++)
            {
                var airDefenceAuras = new Tuple<int, bool>[reader.ReadByte()];
                for (var i = 0; i < airDefenceAuras.Length; i++)
                {
                    var tup = new Tuple<int, bool>(reader.ReadSByte(), reader.ReadBoolean());
                    airDefenceAuras[i] = tup;
                }
            }
            else
            {
                throw new Exception();
            }


            if (reader.ReadByte() == curse++)
            {
                var effects = new Tuple<int, string, string>[reader.ReadByte()];
                for (var i = 0; i < effects.Length; i++)
                {
                    var id = reader.ReadInt16();
                    var name = reader.ReadString();
                    var node = reader.ReadString();
                    var tup = new Tuple<int, string, string>(id, name, node);
                    effects[i] = tup;
                }
            }
            else
            {
                throw new Exception();
            }

            uint shipId = 0;


            if (reader.ReadByte() == curse++)
                switch (reader.ReadByte())
                {
                    case 0x00:
                        //Empty dict

                        break;
                    case 0x01:
                        // not empty dict
                        shipId = reader.ReadUInt32();
                        ReadBlob(reader, false);
                        break;
                    default:
                        throw new Exception();
                        break;
                }
        }

        private void BasePlayerCrate(BinaryReader reader)
        {
            var entityId = reader.ReadInt32();
            var entityType = reader.ReadInt16();
            AvatarId = entityId;
        }

        private void DecodeMap(BinaryReader binaryReader)
        {
            var spaceId = binaryReader.ReadInt32();
            var arenaId = binaryReader.ReadInt64();

            var hex = binaryReader.ReadBytes(17 * 8);

            var nameLength = binaryReader.ReadInt32();
            var name = binaryReader.ReadBytes(nameLength);

            var nameString = Encoding.ASCII.GetString(name);

            if (!nameString.Contains("spaces/")) throw new Exception($"Unknown map name {nameString}");

            nameString = nameString.Substring("spaces/".Length);

            var nameSplit = nameString.Split(new[] {'_'}, 2);
            if (nameSplit.Length != 2) throw new Exception($"Unknown to split map {nameString} to id and name.");

            if (!int.TryParse(nameSplit[0], out var id))
                throw new Exception($"Unable to convert {nameSplit[0]} to id.");


            Map = new Map(id, nameSplit[1], arenaId, _mapIdConvertRatio[id], spaceId);
        }


        private void CellPlayerCreate(BinaryReader data)
        {
            var entityId = data.ReadInt32();
            var spaceId = data.ReadInt32();
            var vehicleId = data.ReadInt32();
            var position = Read3D(data);
            var direction = Read3D(data);


//            var ply = new Player();
            var valueSize = data.ReadUInt32();
            var hex = data.ReadBytes((int) valueSize);
        }

        private void DecodeControlPlayerPosition(float time, BinaryReader reader)
        {
            var id1 = reader.ReadInt32();
            var id2 = reader.ReadInt32();

            if (id2 == 0 && id1 != AvatarId)
            {
                var position = Read3D(reader);
                var rotation = Read3D(reader);

                var vehicle = EntityIdVehicle[id1] ?? throw new Exception();


                PositionRecords.Add(new PositionRecord(time, vehicle, position, rotation));
            }
        }

        private Matrix3 Read3D(BinaryReader data)
        {
            var x = data.ReadSingle();
            var y = data.ReadSingle();
            var z = data.ReadSingle();
            return new Matrix3(x, y, z);
        }

        private dynamic ReadBlob(BinaryReader reader, bool load = true)
        {
            var length = Convert.ToInt32(reader.ReadByte());
            if (length == 0xFF)
            {
                length = reader.ReadUInt16();
                //Skip 1 useless byte
                reader.Read();
            }

            if (load)
                using (var pkl = new Unpickler())
                {
                    var pObject = pkl.loads(reader.ReadBytes(length));
                    return pObject;
                }

            return null;
        }
    }
}