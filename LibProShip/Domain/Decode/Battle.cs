using System;
using System.Collections.Generic;

namespace LibProShip.Domain.Decode
{
    public class Vehicle : ValueObject<Vehicle>
    {
        public Vehicle(long id, int teamId, long shipId, Player player)
        {
            this.Id = id;
            TeamId = teamId;
            ShipId = shipId;
            Player = player;
        }

        public Vehicle()
        {
        }


        public int TeamId { get; private set; }

        public long ShipId { get; private set; }

        public Player Player { get; private set; }


        public bool SameAs(Vehicle other)
        {
            throw new NotImplementedException();
        }
    }

    public class Battle : ValueObject<Battle>
    {
        public Battle(string version, int duration, Player controlPlayer, ICollection<Vehicle> vehicles)
        {
            Version = version;
            Duration = duration;
            ControlPlayer = controlPlayer;
            Vehicles = vehicles;
        }

        public Battle()
        {
        }

        public ICollection<Vehicle> Vehicles { get; private set; }
        public Player ControlPlayer { get; private set; }
        public string Version { get; private set; }
        public int Duration { get; private set; }


        public bool SameAs(Battle other)
        {
            throw new NotImplementedException();
        }
    }
}