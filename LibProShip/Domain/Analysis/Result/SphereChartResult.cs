using System.Collections.Generic;
using LibProShip.Domain.Analysis.Result;

namespace LibProShip.Domain.Analysis.Result
{
    public sealed class SphereChartResult : ValueObject,IAnalysisResult
    {
        public SphereChartResult()
        {
            //For ORM
        }

        public SphereChartResult(IDictionary<int, SpotSample> samples)
        {
            this.Samples = samples;
        }

        public IDictionary<int, SpotSample> Samples { get; private set; }
    }
}