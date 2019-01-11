using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using LibProShip.Domain.StreamProcessor.Packet;
using LibProShip.Infrastructure.Unpickling;

namespace LibProShip.Domain.StreamProcessor.Version
{
    public sealed class DefaultStreamProcessor : IStreamProcessor
    {
        public DefaultStreamProcessor()
        {
        }


        public BattleRecord ProcessStream(byte[] data)
        {
            var p = new InnerProcessor(data);
            return p.GetRecord();
        }
    }


    internal class InnerProcessor
    {
        private readonly byte[] Data;
        private ICollection<Vehicle> Enemy { get; set; }
        private ICollection<Vehicle> Alies { get; set; }

        private ICollection<Vehicle> Vehicles => this.Enemy.Concat(this.Alies).ToList();

        private IList<PositionRecord> PositionRecords { get; set; }
        public IList<DamageRecord> DamageRecords { get; set; }
        private IList<GunShootRecord> GunShootRecords { get; set; }
        private IList<HitRecord> HitRecords { get; set; }
        private IList<TorpedoShootRecord> TorpedoShootRecords { get; set; }
        private BattleRecord Res { get; set; }
        private IDictionary<int, Player> EntityIdPlayer { get; set; }
        private IDictionary<int, Vehicle> EntityIdVehicle { get; set; }
        private int AvatarId { get; set; }
        private List<int> ShipEntityIds { get; set; }
        public Map Map { get; private set; }


        public InnerProcessor(byte[] data)
        {
            this.Data = data;
            this.ShipEntityIds = new List<int>();
            this.Alies = new List<Vehicle>();
            this.Enemy = new List<Vehicle>();
            this.EntityIdPlayer = new Dictionary<int, Player>();
            this.EntityIdVehicle = new Dictionary<int, Vehicle>();
            this.PositionRecords = new List<PositionRecord>();
            this.DamageRecords = new List<DamageRecord>();
            this.GunShootRecords = new List<GunShootRecord>();
            this.TorpedoShootRecords = new List<TorpedoShootRecord>();
            this.HitRecords = new List<HitRecord>();
        }

        public BattleRecord GetRecord()
        {
            using (var st = new MemoryStream(this.Data))
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
                                this.BasePlayerCrate(reader);
                                break;
                            case 0x5:
                                this.Entity(reader);
                                break;
                            case 0x8:
                                this.EntityMethod(time, reader);
                                break;
                            case 1:
                                this.CellPlayerCreate(reader);
                                break;
                            case 10:
                                this.DecodeOtherPlayerPosition(time, reader);
                                break;
                            case 43:
                                this.DecodeControlPlayerPosition(time, reader);
                                break;
                            case 0x27:
                                this.DecodeMap(reader);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }


            if (Res == null)
            {
                this.Res = new BattleRecord(this.ArenaId, this.Map, this.ControlVehicle, this.EntityIdPlayer.Values,
                    this.Vehicles,
                    this.PositionRecords, this.TorpedoShootRecords, this.GunShootRecords, this.HitRecords,
                    this.DamageRecords);
            }

            return Res;
        }

        private void DecodeOtherPlayerPosition(float time, BinaryReader reader)
        {
            var entityId = Convert.ToInt32(reader.ReadUInt32());

            if (reader.BaseStream.Length > 45)
            {
                var spaceId = Convert.ToInt32(reader.ReadUInt32());
            }

            var vehicleId = Convert.ToInt32(reader.ReadUInt32());
            var position = this.Read3D(reader);
            var positionError = this.Read3D(reader);
            var rotation = this.Read3D(reader);
            var isErrored = reader.ReadBoolean();

            if (!this.EntityIdVehicle.ContainsKey(entityId))
            {
                return;
            }

            var vehicle = this.EntityIdVehicle[entityId] ?? throw new Exception();
            this.PositionRecords.Add(new PositionRecord(time, vehicle, position, rotation));
        }

        private void EntityMethod(float time, BinaryReader reader)
        {
            var entityId = Convert.ToInt32(reader.ReadUInt32());
            var messageId = Convert.ToInt32(reader.ReadUInt32());

            if (entityId == this.AvatarId)
            {
                switch (messageId)
                {
                    //Avatar 
                    case 84:
                        this.AddPlayer(reader);
                        break;
                    case 73:
                        this.AddGunAndTorpedo(time, reader);
                        break;
                    default:
                        break;
                }
            }
            else if (this.Vehicles.Select(x => x.VehicleId).Contains(entityId))
            {
                switch (messageId)
                {
                    case 51:
                        this.AddDamage(time, entityId, reader);
                        break;
                }
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

                this.DamageRecords.Add(new DamageRecord(time, this.EntityIdVehicle[vehicleId],
                    this.EntityIdVehicle[entityId], damageAmount));
            }
        }

        private void AddGunAndTorpedo(float time, BinaryReader reader)
        {
            var dataLength = reader.ReadInt32();
            var shotLength = Convert.ToInt32(reader.ReadByte());
            for (var i = 0; i < shotLength; i++)
            {
                this.AddGun(time, reader);
            }

            var torpedoLength = Convert.ToInt32(reader.ReadByte());
            for (var i = 0; i < torpedoLength; i++)
            {
                this.AddTorpedo(time, reader);
            }

            var hitLength = Convert.ToInt32(reader.ReadByte());
            for (var i = 0; i < hitLength; i++)
            {
                this.AddHit(time, reader);
            }
        }

        private void AddHit(float time, BinaryReader reader)
        {
            var pos = this.Read3D(reader);
            var ownerId = reader.ReadInt32();
            var shotId = Convert.ToInt32(reader.ReadUInt16());
            var hitType = Convert.ToInt32(reader.ReadByte());

            var vehicle = this.Vehicles.FirstOrDefault(x => x.VehicleId == ownerId) ?? throw new Exception();

            this.HitRecords.Add(new HitRecord(vehicle, time, pos, shotId, hitType));
        }


        private void AddTorpedo(float time, BinaryReader reader)
        {
            var gameparamsId = reader.ReadUInt32();
            var pos = this.Read3D(reader);
            var dir = this.Read3D(reader);
            var ownerId = reader.ReadInt32();
            var salvoId = reader.ReadInt32();
            var shotId = Convert.ToInt32(reader.ReadUInt16());
            var skinId = reader.ReadUInt32();

            var vehicle = this.Vehicles.FirstOrDefault(x => x.VehicleId == ownerId) ?? throw new Exception();


            this.TorpedoShootRecords.Add(new TorpedoShootRecord(vehicle, time, shotId, salvoId, pos, dir));
        }

        private void AddGun(float time, BinaryReader reader)
        {
            var gameparamsId = reader.ReadUInt32();
            var pos = this.Read3D(reader);
            var dir = this.Read3D(reader);
            var tarPos = this.Read3D(reader);
            var ownerId = reader.ReadInt32();
            var salvoId = reader.ReadInt32();
            var shotId = Convert.ToInt32(reader.ReadUInt16());
            var gunBarrelId = Convert.ToInt32(reader.ReadByte());
            var serverTimeLeft = reader.ReadSingle();
            var shooterHeight = reader.ReadSingle();
            var hitDistance = reader.ReadSingle();

            var vehicle = this.Vehicles.FirstOrDefault(x => x.VehicleId == ownerId) ?? throw new Exception();


            this.GunShootRecords.Add(new GunShootRecord(vehicle, time, shotId, salvoId, pos, dir, tarPos,
                hitDistance, gunBarrelId));
        }

        private void AddPlayer(BinaryReader reader)
        {
            var length = reader.ReadInt32();

            var areadId = reader.ReadInt64();
            this.ArenaId = areadId;
            var teamBuiltId = Convert.ToInt32(reader.ReadByte());
            var playersStates = this.ReadBlob(reader);

            foreach (var playersState in playersStates)
            {
                var name = playersState[20][1];
                var id = playersState[0][1];
                var vehicleId = playersState[27][1];
                var shipId = playersState[29][1];
                var player = new Player(name, id, shipId);
                var team = playersState[30][1];
                var avatarId = playersState[1][1];
                this.EntityIdPlayer[avatarId] = player;

                var vehicle = new Vehicle(vehicleId, player);
                this.EntityIdVehicle[vehicleId] = vehicle;
                this.EntityIdVehicle[avatarId] = vehicle;

                if (avatarId == this.AvatarId)
                {
                    this.ControlVehicle = vehicle;
                }

                switch (team)
                {
                    case 0:
                        this.Alies.Add(vehicle);
                        break;
                    case 1:
                        this.Enemy.Add(vehicle);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        private Vehicle ControlVehicle { get; set; }

        public long ArenaId { get; set; }

        private void Entity(BinaryReader reader)
        {
            var entityId = reader.ReadInt32();
            var entityType = reader.ReadInt16();
            var spaceId = reader.ReadUInt32();
            var vehicleId = reader.ReadInt32();
            var position = this.Read3D(reader);
            var rotation = this.Read3D(reader);

            switch (entityType)
            {
                case 2:
                    this.ShipEntityIds.Add(entityId);
                    this.VehicleEntityCreate(entityId, reader);


                    break;
                default:
                    break;
            }
        }

        private void VehicleEntityCreate(int entityId, BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var count = Convert.ToInt32(reader.ReadByte());

            if (count != 37)
            {
                throw new Exception();
            }

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
                for (var i = 0; i < atbaTargets.Length; i++)
                {
                    atbaTargets[i] = reader.ReadUInt32();
                }
            }
            else
            {
                throw new Exception();
            }

            var airDefenceTargetIds = new long[0];
            if (reader.ReadByte() == curse++)
            {
                airDefenceTargetIds = new long[reader.ReadByte()];
                for (var i = 0; i < airDefenceTargetIds.Length; i++)
                {
                    airDefenceTargetIds[i] = reader.ReadInt64();
                }
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
                for (int i = 0; i < effects.Length; i++)
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
            {
                switch (reader.ReadByte())
                {
                    case 0x00:
                        //Empty dict

                        break;
                    case 0x01:
                        // not empty dict
                        shipId = reader.ReadUInt32();
                        this.ReadBlob(reader, false);
                        break;
                    default:
                        throw new Exception();
                        break;
                }
            }
        }

        private void BasePlayerCrate(BinaryReader reader)
        {
            var entityId = reader.ReadInt32();
            var entityType = reader.ReadInt16();
            this.AvatarId = entityId;
        }

        private void DecodeMap(BinaryReader binaryReader)
        {
            var spaceId = binaryReader.ReadInt32();
            var arenaId = binaryReader.ReadInt32();

            this.Map = new Map(arenaId, spaceId);
        }


        private void CellPlayerCreate(BinaryReader data)
        {
            var entityId = data.ReadInt32();
            var spaceId = data.ReadInt32();
            var vehicleId = data.ReadInt32();
            var position = this.Read3D(data);
            var direction = this.Read3D(data);


//            var ply = new Player();
            var valueSize = data.ReadUInt32();
            var hex = data.ReadBytes((int) valueSize);
        }

        private void DecodeControlPlayerPosition(float time, BinaryReader reader)
        {
            var id1 = reader.ReadInt32();
            var id2 = reader.ReadInt32();

            if (id2 == 0 && id1 != this.AvatarId)
            {
                var position = this.Read3D(reader);
                var rotation = this.Read3D(reader);

                var vehicle = this.EntityIdVehicle[id1] ?? throw new Exception();


                this.PositionRecords.Add(new PositionRecord(time, vehicle, position, rotation));
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
            {
                using (var pkl = new Unpickler())
                {
                    var pObject = pkl.loads(reader.ReadBytes(length));
                    return pObject;
                }
            }
            else
            {
                return null;
            }
        }
    }
}