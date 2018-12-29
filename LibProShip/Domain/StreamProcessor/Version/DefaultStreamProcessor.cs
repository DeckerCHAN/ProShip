using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using LibProShip.Domain.StreamProcessor.Packet;

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
        public List<Player> Players = new List<Player>();
        public List<PositionRecord> PositionRecords = new List<PositionRecord>();
        public BattleRecord Res { get; private set; }
        private int AvatarId;
        public Map Map { get; private set; }

        public InnerProcessor(byte[] data)
        {
            this.Data = data;
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
                            case 1:
                                this.CellPlayerCreate(reader);
                                break;
                            case 34:
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
            
            
            var ply = new Player();
            var valueSize = data.ReadUInt32();
            var hex = data.ReadBytes((int) valueSize);
        }

        private void DecodeOtherPosition(float time, BinaryReader data)
        {
            var id1 = data.ReadInt32();
            var id2 = data.ReadInt32();
            
//            this.PositionRecords.Add(new PositionRecord());
            throw new NotImplementedException();
        }

        private Matrix3 Read3D(BinaryReader data)
        {
            var x = data.ReadSingle();
            var y = data.ReadSingle();
            var z = data.ReadSingle();
            return new Matrix3(x, y, z);
        }
    }
}