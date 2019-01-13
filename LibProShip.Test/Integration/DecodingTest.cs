using System.IO;
using LibProShip.Domain.Decoding.Decoder;
using Xunit;

namespace LibProShip.Test.Integration
{
    public class DecodingTest
    {
        [Fact]
        public void Test1()
        {
            var decoder = new BlowFishDecoder();

            var comb = decoder.DecodeReplay(new FileInfo(@"D:\World_of_Warships\replays\20190113_154142_PBSD105-Acasta_01_solomon_islands.wowsreplay"));

            using (var file = new FileStream("binaryFileForTest.bin", FileMode.Create))
            {
                file.Write(comb.Item2, 0, comb.Item2.Length);
                file.Flush();
            }
        }
    }
}