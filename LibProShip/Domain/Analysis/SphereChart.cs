using System.Collections.Generic;

namespace LibProShip.Domain.Analysis
{
    public sealed class SphereChart : ValueObject
    {
        public SphereChart()
        {
            //For ORM
        }

        public SphereChart(IDictionary<int, SphereSample> samples)
        {
            this.Samples = samples;
        }

        public IDictionary<int, SphereSample> Samples { get; private set; }
    }
}