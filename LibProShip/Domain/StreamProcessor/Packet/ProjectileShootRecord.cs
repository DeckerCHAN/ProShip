namespace LibProShip.Domain.StreamProcessor.Packet
{
    public abstract class ProjectileShootRecord
    {
        protected ProjectileShootRecord(Vehicle ownerVehicle, float shootTime, int shotId, int salvoId,
            Matrix3 position,
            Matrix3 direction)
        {
            this.OwnerVehicle = ownerVehicle;
            this.ShootTime = shootTime;
            this.ShotId = shotId;
            this.SalvoId = salvoId;
            this.Position = position;
            this.Direction = direction;
        }

        
        public static bool operator ==(ProjectileShootRecord o1, ProjectileShootRecord o2)
        {
            return o1?.Equals((object)o2) ?? object.ReferenceEquals(o2, null);
        }

        public static bool operator !=(ProjectileShootRecord o1, ProjectileShootRecord o2)
        {
            return !(o1 == o2);
        }

        
        
        protected bool Equals(ProjectileShootRecord other)
        {
            return Equals(this.OwnerVehicle, other.OwnerVehicle) && this.ShotId == other.ShotId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectileShootRecord) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.OwnerVehicle != null ? this.OwnerVehicle.GetHashCode() : 0) * 397) ^ this.ShotId;
            }
        }

        public Vehicle OwnerVehicle { get; }
        public float ShootTime { get; }
        public int ShotId { get; }
        public int SalvoId { get; }
        public Matrix3 Position { get; }
        public Matrix3 Direction { get; }
    }
}