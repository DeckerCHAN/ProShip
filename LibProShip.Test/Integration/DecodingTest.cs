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

            var comb = decoder.DecodeReplay(new FileInfo("G:\\Python\\replays_unpack\\replays\\20181117_011834_PRSD210-Grozovoy-pr-40N_01_solomon_islands.wowsreplay"));

            using (var file = new FileStream("binaryFileForTest.bin", FileMode.Create))
            {
                file.Write(comb.Item2, 0, comb.Item2.Length);
                file.Flush();
            }
        }
    }
}