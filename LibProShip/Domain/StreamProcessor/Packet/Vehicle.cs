namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Vehicle
    {
        public Vehicle(int vehicleId, Player controlPlayer)
        {
            this.ControlPlayer = controlPlayer;
            this.VehicleId = vehicleId;
        }

        public int VehicleId { get; }
        public Player ControlPlayer { get; }

        private bool Equals(Vehicle other)
        {
            return this.VehicleId == other.VehicleId && Equals(this.ControlPlayer, other.ControlPlayer);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Vehicle other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.VehicleId * 397) ^ (this.ControlPlayer != null ? this.ControlPlayer.GetHashCode() : 0);
            }
        }

 
        public static bool operator ==(Vehicle o1, Vehicle o2)
        {
            return o1?.Equals((object)o2) ?? object.ReferenceEquals(o2, null);
        }

        public static bool operator !=(Vehicle o1, Vehicle o2)
        {
            return !(o1 == o2);
        }

   

        public override string ToString()
        {
            return $"[{this.VehicleId}]{this.ControlPlayer.Name}";
        }
    }
}