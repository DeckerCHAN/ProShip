namespace LibProShip.Domain.StreamProcessor.Packet
{
    public class TorpedoShootRecord : ProjectileShootRecord
    {
        public TorpedoShootRecord(Vehicle ownerVehicle, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction) : base(ownerVehicle, shootTime, shotId, salvoId, position, direction)
        {
        }
    }
}