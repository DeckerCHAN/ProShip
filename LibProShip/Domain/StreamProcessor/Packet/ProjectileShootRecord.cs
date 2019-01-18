namespace LibProShip.Domain.StreamProcessor.Packet
{
    public abstract class ProjectileShootRecord
    {
        protected ProjectileShootRecord(Vehicle ownerVehicle, float shootTime, int shotId, int salvoId,
            Matrix3 position,
            Matrix3 direction)
        {
            OwnerVehicle = ownerVehicle;
            ShootTime = shootTime;
            ShotId = shotId;
            SalvoId = salvoId;
            Position = position;
            Direction = direction;
        }

        public Vehicle OwnerVehicle { get; }
        public float ShootTime { get; }
        public int ShotId { get; }
        public int SalvoId { get; }
        public Matrix3 Position { get; }
        public Matrix3 Direction { get; }


        public static bool operator ==(ProjectileShootRecord o1, ProjectileShootRecord o2)
        {
            return o1?.Equals((object) o2) ?? ReferenceEquals(o2, null);
        }

        public static bool operator !=(ProjectileShootRecord o1, ProjectileShootRecord o2)
        {
            return !(o1 == o2);
        }


        protected bool Equals(ProjectileShootRecord other)
        {
            return Equals(OwnerVehicle, other.OwnerVehicle) && ShotId == other.ShotId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ProjectileShootRecord) obj);
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