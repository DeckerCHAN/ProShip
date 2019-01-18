using System;
using System.Collections.Generic;

namespace LibProShip.Domain.Replay.Entities
{
    public class Vehicle : ValueObject
    {
        public Vehicle(int teamId, long shipId, Player player)
        {
            TeamId = teamId;
            ShipId = shipId;
            Player = player;
        }

        public Vehicle()
        {
        }


        public int TeamId { get; }

        public long ShipId { get; }

        public Player Player { get; }
    }

    public class Battle : ValueObject
    {
        public Battle(string version, int duration, Vehicle controlVehicle, ICollection<Vehicle> vehicles,
            DateTime dateTime)
        {
            Version = version;
            Duration = duration;
            ControlVehicle = controlVehicle;
            Vehicles = vehicles;
            DateTime = dateTime;
        }

        public Battle()
        {
            //For ORM
        }

        public ICollection<Vehicle> Vehicles { get; }
        public Vehicle ControlVehicle { get; }
        public string Version { get; }
        public int Duration { get; }
        public DateTime DateTime { get; }
    }
}