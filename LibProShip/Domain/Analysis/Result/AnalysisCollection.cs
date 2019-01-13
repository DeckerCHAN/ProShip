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
            this.Properties = properties;
            this.SphereChartses = sphereCharts;
        }

        public AnalysisCollection()
        {
            //For ORM
        }

        public IDictionary<string, string> Properties { get; private set; }
        public IEnumerable<SphereChartResult> SphereChartses { get; private set; }
    }
}