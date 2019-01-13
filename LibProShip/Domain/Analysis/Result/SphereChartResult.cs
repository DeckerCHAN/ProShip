using System;
using System.Collections.Generic;

namespace LibProShip.Domain.Analysis.Result
{
    public sealed class SphereChartResult : ValueObject, IAnalysisResult
    {
        public SphereChartResult(IEnumerable<SpotSample> spotSamples, IEnumerable<PointSample> pointSamples, string title)
        {
            SpotSamples = spotSamples;
            PointSamples = pointSamples;
            Title = title;
        }

        public SphereChartResult()
        {
            //For ORM
        }

        public string Title { get; private set; }
        public IEnumerable<SpotSample> SpotSamples { get; private set; }
        public IEnumerable<PointSample> PointSamples { get; private set; }
    }
}