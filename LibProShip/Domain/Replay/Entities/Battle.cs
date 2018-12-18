using System;
using System.Collections.Generic;

namespace LibProShip.Domain.Replay.Entities
{
    public class Vehicle : ValueObject
    {
        public Vehicle( int teamId, long shipId, Player player)
        {
            this.TeamId = teamId;
            this.ShipId = shipId;
            this.Player = player;
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

    public class Battle : ValueObject
    {
        public Battle(string version, int duration, Player controlPlayer, ICollection<Vehicle> vehicles)
        {
            this.Version = version;
            this.Duration = duration;
            this.ControlPlayer = controlPlayer;
            this.Vehicles = vehicles;
        }

        public Battle()
        {
        }

        public ICollection<Vehicle> Vehicles { get; private set; }
        public Player ControlPlayer { get; private set; }
        public string Version { get; private set; }
        public int Duration { get; private set; }

    }
}