using System.Collections.Generic;

namespace LibProShip.Domain.Analysis.Result
{
    public sealed class SphereChartResult : ValueObject, IAnalysisResult
    {
        public SphereChartResult(IEnumerable<SpotSample> spotSamples, IEnumerable<PointSample> pointSamples)
        {
            SpotSamples = spotSamples;
            PointSamples = pointSamples;
        }

        public SphereChartResult()
        {
            //For ORM
        }

        public IEnumerable<SpotSample> SpotSamples { get; private set; }
        public IEnumerable<PointSample> PointSamples { get; private set; }
    }
}