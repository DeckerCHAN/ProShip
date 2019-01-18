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

            var comb = decoder.DecodeReplay(new FileInfo(
                @"C:\Users\derek\Downloads\2807_1547589287_20190115_223151_PBSC110-Minotaur_16_OC_bees_to_honey.wowsreplay"));

            using (var file = new FileStream("binaryFileForTest.bin", FileMode.Create))
            {
                file.Write(comb.Item2, 0, comb.Item2.Length);
                file.Flush();
            }
        }
    }
}