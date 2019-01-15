using LibProShip.Domain.StreamProcessor.Packet;
using Xunit;

namespace LibProShip.Test.Unit.MustRun
{
    public class VehicleTest
    {
        [Fact]
        public void VehiclesShouldEqual()
        {
            var v1 = new Vehicle(00001, new Player("TestPlayer", 1000), 5);
            var v2 = new Vehicle(00001, new Player("TestPlayer", 1000), 5);

            Assert.Equal(v1, v2);
            Assert.True(v1 == v2);
        }
    }
}