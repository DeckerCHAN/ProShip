namespace LibProShip.Domain.StreamProcessor.Packet
{
    public enum HitType
    {
        OutOfRange = 0,
        Miss = 4,
        HitOnTheMountain = 8,
        Hit = 12
    }

    public sealed class HitRecord
    {
        public HitRecord(Vehicle ownerVehicle, float hitTime, Matrix3 hitPosition, int shotId, HitType hitType)
        {
            HitPosition = hitPosition;
            OwnerVehicle = ownerVehicle;
            ShotId = shotId;
            HitType = hitType;
            HitTime = hitTime;
        }

        public float HitTime { get; }
        public Matrix3 HitPosition { get; }
        public Vehicle OwnerVehicle { get; }
        public int ShotId { get; }
        public HitType HitType { get; }


        public static bool operator ==(HitRecord o1, HitRecord o2)
        {
            return o1?.Equals((object) o2) ?? ReferenceEquals(o2, null);
        }

        public static bool operator !=(HitRecord o1, HitRecord o2)
        {
            return !(o1 == o2);
        }

        private bool Equals(HitRecord other)
        {
            return Equals(OwnerVehicle, other.OwnerVehicle) && ShotId == other.ShotId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is HitRecord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((OwnerVehicle != null ? OwnerVehicle.GetHashCode() : 0) * 397) ^ ShotId;
            }
        }
    }
}