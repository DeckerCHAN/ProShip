using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public IList<Player> Enemy;
        public IList<Player> Alies;
        public IList<PositionRecord> PositionRecords;
        public IList<GunShootShootRecord> GunShootShootRecords;
        public BattleRecord Res { get; private set; }
        private IDictionary<int, Player> EntityIdPlayer;
        private int AvatarId;
        private List<int> ShipEntityIds;
        public Map Map { get; private set; }

        public InnerProcessor(byte[] data)
        {
            this.Data = data;
            this.ShipEntityIds = new List<int>();
            this.Alies = new List<Player>();
            this.Enemy = new List<Player>();
            this.EntityIdPlayer = new Dictionary<int, Player>();
            this.PositionRecords = new List<PositionRecord>();
            this.GunShootShootRecords = new List<GunShootShootRecord>();
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
                            case 43:
                                this.DecodeOtherPosition(time, reader);
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

            throw new NotImplementedException();
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
            for (int i = 0; i < torpedoLength; i++)
            {
                this.AddTorpedo(time, reader);
            }

            var hitLength = Convert.ToInt32(reader.ReadByte());
            for (int i = 0; i < hitLength; i++)
            {
                this.AddHit(time, reader);
            }
            throw new NotImplementedException();
        }

        private void AddHit(float time, BinaryReader reader)
        {
            throw new NotImplementedException();
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

            var player = this.EntityIdPlayer[ownerId];

            this.GunShootShootRecords.Add(new GunShootShootRecord(player, time, shotId, salvoId, pos, dir, tarPos,
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
                var shipId = playersState[27][1];
                var player = new Player(name, id, shipId);
                var team = playersState[30][1];
                var avatarId = playersState[1][1];
                this.EntityIdPlayer[avatarId] = player;
                switch (team)
                {
                    case 0:
                        this.Alies.Add(player);
                        break;
                    case 1:
                        this.Enemy.Add(player);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported team id {team}");
                }
            }
        }

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
                    this.VehicleEntityCreate(reader);


                    break;
                default:
                    break;
            }
        }

        private void VehicleEntityCreate(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var count = Convert.ToInt32(reader.ReadChar());

            if (count != 37)
            {
                throw new Exception();
            }

            var curse = 0;

            if (Convert.ToInt32(reader.ReadByte()) != curse++)
            {
                throw new Exception();
            }

            var isAntiAirMode = reader.ReadBoolean();

            if (Convert.ToInt32(reader.ReadByte()) != curse++)
            {
                throw new Exception();
            }

            var burningFlag = Convert.ToInt32(reader.ReadByte());

            if (Convert.ToInt32(reader.ReadByte()) != curse++)
            {
                throw new Exception();
            }

            var buoyancyCurrentState = Convert.ToInt32(reader.ReadByte());

            if (Convert.ToInt32(reader.ReadByte()) != curse++)
            {
                throw new Exception();
            }

            var isOnForsage = reader.ReadBoolean();
            if (Convert.ToInt32(reader.ReadByte()) != curse++)
            {
                throw new Exception();
            }

            var teamId = Convert.ToInt32(reader.ReadByte());

            if (Convert.ToInt32(reader.ReadByte()) != curse++)
            {
                throw new Exception();
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

        private void DecodeOtherPosition(float time, BinaryReader reader)
        {
            var id1 = reader.ReadInt32();
            var id2 = reader.ReadInt32();

            if (id2 == 0)
            {
                var position = this.Read3D(reader);
                var rotation = this.Read3D(reader);

                this.PositionRecords.Add(new PositionRecord(time, this.EntityIdPlayer[id1], position, rotation));
            }
            else
            {
                // Projection packet
                this.EntityIdPlayer[id2] = this.EntityIdPlayer[id1];
            }
        }

        private Matrix3 Read3D(BinaryReader data)
        {
            var x = data.ReadSingle();
            var y = data.ReadSingle();
            var z = data.ReadSingle();
            return new Matrix3(x, y, z);
        }

        private dynamic ReadBlob(BinaryReader reader)
        {
            var length = Convert.ToInt32(reader.ReadByte());
            if (length == 0xFF)
            {
                length = reader.ReadUInt16();
                //Skip 1 useless byte
                reader.Read();
            }

            using (var pkl = new Unpickler())
            {
                var pObject = pkl.loads(reader.ReadBytes(length));
                return pObject;
            }
        }
    }
}