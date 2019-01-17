using System.Linq;
using LibProShip.StaticResources;
using Xunit;

namespace LibProShip.Test.Unit
{
    public class ResourceTest
    {
        [Fact]
        public void Test1()
        {
            var selectedShips = ShipDefinition.ShipShipDefinitions
                    .Where(x => x.Tier == 6)
                    .Where(x => x.ShipType == ShipType.AirCarrier)
                    .Select(x => x.ShipName).ToArray();

            Assert.Contains("Independence", selectedShips);
            Assert.Contains("Ryujo", selectedShips);
            
        }
    }
}