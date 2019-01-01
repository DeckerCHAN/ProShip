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
        public IList<Player> Enemy = new List<Player>();
        public IList<Player> Alies = new List<Player>();
        public List<PositionRecord> PositionRecords = new List<PositionRecord>();
        public BattleRecord Res { get; private set; }
        private IDictionary<int, Player> EntityIdPlayer = new Dictionary<int, Player>();
        private int AvatarId;
        private List<int> ShipEntityIds;
        public Map Map { get; private set; }

        public InnerProcessor(byte[] data)
        {
            this.Data = data;
            this.ShipEntityIds = new List<int>();
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
                                this.EntityMethod(reader);
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

        private void EntityMethod(BinaryReader reader)
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
                        this.AddGunAndTorpedo(reader);
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddGunAndTorpedo(BinaryReader reader)
        {
            var shotLength = Convert.ToInt32(reader.ReadByte());
            throw new NotImplementedException();
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
                    this.ShipPropertyMap(reader);


                    break;
                default:
                    break;
            }
        }

        private void ShipPropertyMap(BinaryReader reader)
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

            var burningFlag = reader.Read();

            if (Convert.ToInt32(reader.ReadByte()) != curse++)
            {
                throw new Exception();
            }

            var buoyancyCurrentState = reader.Read();

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

        private void DecodeMethod(BinaryReader data)
        {
            var entityId = data.ReadUInt32();
            var methodId = data.ReadUInt32();
        }

        private void DecodePlayers(BinaryReader data)
        {
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