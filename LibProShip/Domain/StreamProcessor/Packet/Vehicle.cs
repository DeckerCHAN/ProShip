namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Vehicle
    {
        public Vehicle(int vehicleId, Player controlPlayer, long shipId)
        {
            ControlPlayer = controlPlayer;
            ShipId = shipId;
            VehicleId = vehicleId;
        }

        public int VehicleId { get; }
        public long ShipId { get; }
        public Player ControlPlayer { get; }

        private bool Equals(Vehicle other)
        {
            return VehicleId == other.VehicleId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Vehicle other && Equals(other);
        }

        public override int GetHashCode()
        {
            return VehicleId;
        }


        public static bool operator ==(Vehicle o1, Vehicle o2)
        {
            return o1?.Equals((object) o2) ?? ReferenceEquals(o2, null);
        }

        public static bool operator !=(Vehicle o1, Vehicle o2)
        {
            return !(o1 == o2);
        }


        public override string ToString()
        {
            return $"[{VehicleId}]{ControlPlayer.Name}";
        }
    }
}