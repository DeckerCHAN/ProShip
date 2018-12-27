using System.Collections.Generic;

namespace LibProShip.Domain.Analysis.Dto
{
    public sealed class SphereChartResult : ValueObject,IAnalysisResult
    {
        public SphereChartResult()
        {
            //For ORM
        }

        public SphereChartResult(IDictionary<int, SphereSample> samples)
        {
            this.Samples = samples;
        }

        public IDictionary<int, SphereSample> Samples { get; private set; }
    }
}