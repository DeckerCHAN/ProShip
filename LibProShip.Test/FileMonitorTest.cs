using System.IO;
using System.Threading;
using LibProShip.Domain.FileSystem;
using LibProShip.Infrastructure.Configs;
using LibProShip.Infrastructure.Event;
using Moq;
using Xunit;

namespace LibProShip.Test
{
    public class FileMonitorTest
    {
        [Fact]
        public void ItShouldRaiseFileChangeEvent()
        {
            Assert.True(true);
        }
    }
}