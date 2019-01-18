using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibProShip.Domain.StreamProcessor;
using LibProShip.Domain.StreamProcessor.Version;
using Xunit;

namespace LibProShip.Test.Integration
{
    public class ProcessorTest
    {
        [Fact]
        public void ItShouldProcessDataFromFileWithoutError()
        {
            IStreamProcessor processor = new DefaultStreamProcessor();
            byte[] data = null;
            using (var st = new FileStream("binaryFileForTest.bin", FileMode.Open))
            {
                data = new byte[st.Length];
                st.Read(data, 0, data.Length);
            }

            var pac = processor.ProcessStream(data);
        }

        [Fact]
        public void StreamProcessTest()
        {
            byte[] data = null;
            using (var st = new FileStream("binaryFileForTest.bin", FileMode.Open))
            {
                data = new byte[st.Length];
                st.Read(data, 0, data.Length);
            }

            var pac = new List<byte[]>();


            var i = 0;
            while (i < data.Length)
            {
                var size = BitConverter.ToInt32(new[] {data[i], data[i + 1], data[i + 2], data[i + 3]}, 0);
                size += 12;

                var ina = new ArraySegment<byte>(data, i, size);
                pac.Add(ina.ToArray());


                i += size;
            }

            var enumerable = pac.Where(x => BitConverter.ToInt32(new ArraySegment<byte>(x, 4, 4).ToArray(), 0) == 43)
                .ToArray();
        }
    }
}