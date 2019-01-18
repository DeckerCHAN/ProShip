using System.Collections.Generic;

namespace LibProShip.Domain.Analysis.Result
{
    public interface IAnalysisResult
    {
    }


    public sealed class AnalysisCollection : ValueObject
    {
        public AnalysisCollection(IDictionary<string, string> properties, IEnumerable<SphereChartResult> sphereCharts)
        {
            Properties = properties;
            SphereChartses = sphereCharts;
        }

        public AnalysisCollection()
        {
            //For ORM
        }

        public IDictionary<string, string> Properties { get; }
        public IEnumerable<SphereChartResult> SphereChartses { get; }
    }
}