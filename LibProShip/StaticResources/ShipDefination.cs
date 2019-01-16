using System.Collections.Generic;

namespace LibProShip.StaticResources
{
    public enum ShipType
    {
        Unknown = 0,
        Destoyer = 1,
        Cusier = 2,
        BattleShip = 3,
        AircraftCarrier = 4,
        Submarine = 5
    }

    public sealed class ShipDefination
    {
        internal ShipDefination(long shipId, string shipName, ShipType shipType)
        {
            ShipId = shipId;
            ShipName = shipName;
            ShipType = shipType;
        }

        public long ShipId { get; }
        public string ShipName { get; }
        public ShipType ShipType { get; }

        public static readonly IEnumerable<ShipDefination> Ships = new ShipDefination[]
        {
        };
    }
}