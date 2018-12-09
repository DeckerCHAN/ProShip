using System;
using System.Collections.Generic;

namespace LibProShip.Domain.Decode
{
    public class Ship : IValueObject<Ship>
    {
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public bool SameAs(Ship other)
        {
            throw new NotImplementedException();
        }
    }

    public class Battle : IValueObject<Battle>
    {
        public string ArenaId { get; }
        public IDictionary<Player, Ship> PlayerShips { get; }
        public Player ControlPlayer { get; }
        public string Version { get; }


        public bool SameAs(Battle other)
        {
            throw new NotImplementedException();
        }
    }
}