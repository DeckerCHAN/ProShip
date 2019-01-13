using LibProShip.Infrastructure.Utils;
using Xunit;

namespace LibProShip.Test.Unit
{
    public class MathUtilsTest
    {
        [Theory]
        [InlineData(0,0,1,1,0.785398D)]
        [InlineData(0,0,1,-1,-0.785398D)]
        [InlineData(0,0,-1,1,2.35619D)]
        [InlineData(0,0,-1,-1,-2.35619D)]
        [InlineData(0,0,1,1.732D,0.523D)]
        public void AngleFormTest(double x1, double y1, double x2, double y2, double expect)
        {
            var res = MathUtils.AngleFrom2D(x1, y1, x2, y2);
            Assert.Equal(res,expect,2);
        }
    }
}