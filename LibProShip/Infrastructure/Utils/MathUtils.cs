using System;

namespace LibProShip.Infrastructure.Utils
{
    public static class MathUtils
    {
        public static double AngleFrom(double x1, double y1, double x2, double y2)
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