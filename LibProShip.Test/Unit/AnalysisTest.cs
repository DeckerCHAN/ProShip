using System.IO;
using LibProShip.Domain.Analysis.Analyser;
using LibProShip.Domain.StreamProcessor;
using LibProShip.Domain.StreamProcessor.Version;
using Xunit;

namespace LibProShip.Test.Unit
{
    public class AnalysisTest
    {
        [Fact]
        public void DamageSphereAnalyserTest()
        {
            //TODO: REMOVE THIS

            IStreamProcessor processor = new DefaultStreamProcessor();
            byte[] data = null;
            using (var st = new FileStream("binaryFileForTest.bin", FileMode.Open))
            {
                data = new byte[st.Length];
                st.Read(data, 0, data.Length);
            }

            var pac = processor.ProcessStream(data);

            //TODO: REMOVE THIS


            var analyer = new DamageSpotAnalyser();
            var result = analyer.Analysis(pac);
        }

        [Fact]
        public void ShellPointAnalyserTest()
        {
            //TODO: REMOVE THIS

            IStreamProcessor processor = new DefaultStreamProcessor();
            byte[] data = null;
            using (var st = new FileStream("binaryFileForTest.bin", FileMode.Open))
            {
                data = new byte[st.Length];
                st.Read(data, 0, data.Length);
            }

            var pac = processor.ProcessStream(data);

            //TODO: REMOVE THIS


            var analyer = new ShellLandingPositionAnalyser();
            var result = analyer.Analysis(pac);
        }
    }
}