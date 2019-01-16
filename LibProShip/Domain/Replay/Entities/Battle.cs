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
    }

    public class Battle : ValueObject
    {
        public Battle(string version, int duration, Vehicle controlVehicle, ICollection<Vehicle> vehicles, DateTime dateTime)
        {
            this.Version = version;
            this.Duration = duration;
            this.ControlVehicle = controlVehicle;
            this.Vehicles = vehicles;
            DateTime = dateTime;
        }

        public Battle()
        {
            //For ORM
        }

        public ICollection<Vehicle> Vehicles { get; private set; }
        public Vehicle ControlVehicle { get; private set; }
        public string Version { get; private set; }
        public int Duration { get; private set; }
        public DateTime DateTime { get; private set; }

    }
}