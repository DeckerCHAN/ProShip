using System;

namespace LibProShip.Domain.StreamProcessor.Packet.Extensions
{
    public static class Matrix3Extension
    {
        public static float DistanceFrom(this Matrix3 position, Matrix3 antherPosition)
        {
            return (float) Math.Sqrt(Math.Pow(position.X - antherPosition.X, 2) +
                                     Math.Pow(position.Y - antherPosition.Y, 2) +
                                     Math.Pow(position.Z - antherPosition.Z, 2));
        }
    }
}