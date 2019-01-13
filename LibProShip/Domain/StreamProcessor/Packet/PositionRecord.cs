namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class PositionRecord
    {
        public PositionRecord(float time, Vehicle vehicle, Matrix3 position, Matrix3 rotation)
        {
            this.Position = position;
            this.Vehicle = vehicle;
            this.Time = time;
            this.Rotation = rotation;
        }

        public float Time { get; }
        public Vehicle Vehicle { get; }
        public Matrix3 Position { get; }
        public Matrix3 Rotation { get; }


        private bool Equals(PositionRecord other)
        {
            return this.Time.Equals(other.Time) && Equals(this.Vehicle, other.Vehicle);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is PositionRecord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Time.GetHashCode() * 397) ^ (this.Vehicle != null ? this.Vehicle.GetHashCode() : 0);
            }
        }


        public static bool operator ==(PositionRecord o1, PositionRecord o2)
        {
            return o1?.Equals((object)o2) ?? object.ReferenceEquals(o2, null);
        }

        public static bool operator !=(PositionRecord o1, PositionRecord o2)
        {
            return !(o1 == o2);
        }

 
    }
}