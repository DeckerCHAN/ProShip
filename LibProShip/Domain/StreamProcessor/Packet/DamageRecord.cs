namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class DamageRecord
    {
        public DamageRecord(float time, Vehicle sourceVehicle, Vehicle targetVehicle, float amount)
        {
            this.Time = time;
            this.SourceVehicle = sourceVehicle;
            this.TargetVehicle = targetVehicle;
            this.Amount = amount;
        }

                
        public static bool operator ==(DamageRecord o1, DamageRecord o2)
        {
            return o1?.Equals((object)o2) ?? object.ReferenceEquals(o2, null);
        }

        public static bool operator !=(DamageRecord o1, DamageRecord o2)
        {
            return !(o1 == o2);
        }
        
        private bool Equals(DamageRecord other)
        {
            return this.Time.Equals(other.Time) && Equals(this.SourceVehicle, other.SourceVehicle) && Equals(this.TargetVehicle, other.TargetVehicle) && this.Amount.Equals(other.Amount);
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
                var hashCode = this.Time.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.SourceVehicle != null ? this.SourceVehicle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.TargetVehicle != null ? this.TargetVehicle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Amount.GetHashCode();
                return hashCode;
            }
        }

        public double Time { get; }
        public Vehicle SourceVehicle { get; }
        public Vehicle TargetVehicle { get; }
        public double Amount { get; }
    }
}