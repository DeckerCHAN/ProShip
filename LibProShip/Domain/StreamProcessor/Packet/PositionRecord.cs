namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class PositionRecord
    {
        public PositionRecord(float time, Vehicle vehicle, Matrix3 position, Matrix3 rotation)
        {
            Position = position;
            Vehicle = vehicle;
            Time = time;
            Rotation = rotation;
        }

        public float Time { get; }
        public Vehicle Vehicle { get; }
        public Matrix3 Position { get; }
        public Matrix3 Rotation { get; }


        private bool Equals(PositionRecord other)
        {
            return Time.Equals(other.Time) && Equals(Vehicle, other.Vehicle);
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
                return (Time.GetHashCode() * 397) ^ (Vehicle != null ? Vehicle.GetHashCode() : 0);
            }
        }


        public static bool operator ==(PositionRecord o1, PositionRecord o2)
        {
            return o1?.Equals((object) o2) ?? ReferenceEquals(o2, null);
        }

        public static bool operator !=(PositionRecord o1, PositionRecord o2)
        {
            return !(o1 == o2);
        }
    }
}