using System;

namespace LibProShip.Infrastructure.Utils
{
    public static class MathUtils
    {
        
        public static double MeanAngle(double r1, double r2)
        {
            var x = (Math.Cos(r1) + Math.Cos(r2)) / 2;
            var y = (Math.Sin(r1) + Math.Sin(r2)) / 2;
            return Math.Atan2(y, x);
        }
        public static double AngleFrom2D(double x1, double y1, double x2, double y2)
        {
            var xDiff = x2 - x1;
            var yDiff = y2 - y1;

            if (xDiff * yDiff < 0)
            {
                var tempDiff = xDiff;
                xDiff = yDiff;
                yDiff = tempDiff;
            }

            return Math.Atan2(xDiff,
                yDiff);
        }
    }
}