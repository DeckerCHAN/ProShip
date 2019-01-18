using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibProShip.Infrastructure;
using Xunit;

namespace LibProShip.Test.Unit
{
    public class BlowFishTest
    {
        public BlowFishTest()
        {
            BlowFish = new Blowfish(new byte[]
                {0x29, 0xB7, 0xC9, 0x09, 0x38, 0x3F, 0x84, 0x88, 0xFA, 0x98, 0xEC, 0x4E, 0x13, 0x19, 0x79, 0xFB});
        }

        public Blowfish BlowFish { get; set; }


        public Stream Decompress(byte[] data)
        {
            var output = new MemoryStream();
            using (var compressedStream = new MemoryStream(new ArraySegment<byte>(data, 2, data.Length - 2).ToArray()))
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                zipStream.CopyTo(output);
                zipStream.Close();
                output.Position = 0;
                return output;
            }
        }


        [Fact]
        public async Task DecipherFile()
        {
            using (var f =
                new FileStream(
                    @"D:\World_of_Warships\replays\20181105_205142_PRSD210-Grozovoy-pr-40N_15_NE_north.wowsreplay",
                    FileMode.Open))
            {
                var signtureBytes = new byte[4];
                f.Read(signtureBytes, 0, 4);

                var blockCountBytes = new byte[4];
                f.Read(blockCountBytes, 0, 4);


                var blockCount = BitConverter.ToInt32(blockCountBytes, 0);

                var blockSizeBytes = new byte [4];
                f.Read(blockSizeBytes, 0, 4);

                var blockSize = BitConverter.ToInt32(blockSizeBytes, 0);


                var areaInfoBytes = new byte [blockSize];
                f.Read(areaInfoBytes, 0, areaInfoBytes.Length);
                var areaInfoString = Encoding.UTF8.GetString(areaInfoBytes);


                var zLibBytes = new byte[f.Length - f.Position];
                f.Read(zLibBytes, 0, zLibBytes.Length);

                var s = zLibBytes.Length % 8;


                var key = new byte[]
                    {0x29, 0xB7, 0xC9, 0x09, 0x38, 0x3F, 0x84, 0x88, 0xFA, 0x98, 0xEC, 0x4E, 0x13, 0x19, 0x79, 0xFB};


                var resStream = new MemoryStream();

                var secondChunk = new ArraySegment<byte>(zLibBytes, 8, 8).ToArray();
                BlowFish.Decipher(secondChunk);
                resStream.Write(secondChunk, 0, secondChunk.Length);

                var previousChunk = new byte[8];
                Array.Copy(secondChunk, previousChunk, 8);


                for (var i = 16; i < zLibBytes.Length; i += 8)
                {
                    var chunk = new ArraySegment<byte>(zLibBytes, i, 8).ToArray();
                    BlowFish.Decipher(chunk);

                    var thisChunkLong = BitConverter.ToInt64(chunk, 0);
                    var previousChunkLong = BitConverter.ToInt64(previousChunk, 0);

                    var xor = thisChunkLong ^ previousChunkLong;
                    var xorbytes = BitConverter.GetBytes(xor);

                    resStream.Write(xorbytes, 0, xorbytes.Length);
                    Array.Copy(xorbytes, previousChunk, 8);
                }


                var resBytes = new byte[resStream.Length];

                resStream.Position = 0;
                resStream.Read(resBytes, 0, resBytes.Length);


                var resST = Decompress(resBytes);


                var a = resST.Length;


//                var msSinkDecompressed = new System.IO.MemoryStream();
//                var zOut = new ZlibStream(msSinkDecompressed, Ionic.Zlib.CompressionMode.Decompress, true);
//                CopyStream(zlibStream, zOut);
//
//                Console.Write(zOut.Length);
//
//                //new Blowfish()
            }
        }

        [Fact]
        public async Task TestEncipherAndDecipher()
        {
            var data = Encoding.ASCII.GetBytes("1234567812345678");
            var encipherData = BlowFish.Encipher(data);
            var decipherData = BlowFish.Decipher(encipherData);
            Assert.Equal("1234567812345678", Encoding.ASCII.GetString(decipherData));
        }
    }
}