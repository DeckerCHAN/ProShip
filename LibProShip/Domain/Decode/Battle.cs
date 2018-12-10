using System;
using System.Collections.Generic;

namespace LibProShip.Domain.Decode
{
    public class Vehicle : ValueObject<Vehicle>
    {
        public Vehicle(string playerName, long playerId, int teamId, long shipId)
        {
            PlayerName = playerName;
            PlayerId = playerId;
            TeamId = teamId;
            ShipId = shipId;
        }

        public long ShipId { get; }
        public int TeamId { get; }
        public long PlayerId { get; }
        public string PlayerName { get; }

        public bool SameAs(Vehicle other)
        {
            throw new NotImplementedException();
        }
    }

    public class Battle : ValueObject<Battle>
    {
        public IReadOnlyDictionary<Player, Vehicle> PlayerShips { get; }
        public Player ControlPlayer { get; }
        public string Version { get; }
        public int Duration { get; }


        public bool SameAs(Battle other)
        {
            throw new NotImplementedException();
        }
    }
}