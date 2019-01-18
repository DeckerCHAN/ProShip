namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class DamageRecord
    {
        public DamageRecord(float time, Vehicle sourceVehicle, Vehicle targetVehicle, float amount)
        {
            Time = time;
            SourceVehicle = sourceVehicle;
            TargetVehicle = targetVehicle;
            Amount = amount;
        }

        public double Time { get; }
        public Vehicle SourceVehicle { get; }
        public Vehicle TargetVehicle { get; }
        public double Amount { get; }


        public static bool operator ==(DamageRecord o1, DamageRecord o2)
        {
            return o1?.Equals((object) o2) ?? ReferenceEquals(o2, null);
        }

        public static bool operator !=(DamageRecord o1, DamageRecord o2)
        {
            return !(o1 == o2);
        }

        private bool Equals(DamageRecord other)
        {
            return Time.Equals(other.Time) && Equals(SourceVehicle, other.SourceVehicle) &&
                   Equals(TargetVehicle, other.TargetVehicle) && Amount.Equals(other.Amount);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DamageRecord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Time.GetHashCode();
                hashCode = (hashCode * 397) ^ (SourceVehicle != null ? SourceVehicle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TargetVehicle != null ? TargetVehicle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Amount.GetHashCode();
                return hashCode;
            }
        }
    }
}