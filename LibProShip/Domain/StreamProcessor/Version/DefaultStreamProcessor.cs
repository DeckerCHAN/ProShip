using System;
using System.IO;

namespace LibProShip.Domain.StreamProcessor.Version
{
    public class DefaultStreamProcessor : IStreamProcessor
    {
        public Packets.Packets ProcessStream(byte[] data)
        {
            using (var st = new MemoryStream(data))
            using (var binaryReader = new BinaryReader(st))
            {
                while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                {
                    var size = binaryReader.ReadInt32();
                    var type = binaryReader.ReadInt32();
                    var time = binaryReader.ReadSingle();
                }
            }

            throw new NotImplementedException();
        }
    }
}