namespace LibProShip.Domain.StreamProcessor.Packet
{
    public class GunShootRecord : ProjectileShootRecord
    {
        public GunShootRecord(Vehicle ownerVehicle, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction, Matrix3 targetPosition, float hitDistance, int gunBarrelId) : base(ownerVehicle,
            shootTime,
            shotId, salvoId, position, direction)
        {
            TargetPosition = targetPosition;
            HitDistance = hitDistance;
            GunBarrelId = gunBarrelId;
        }

        public Matrix3 TargetPosition { get; }
        public float HitDistance { get; }
        public int GunBarrelId { get; }
    }
}