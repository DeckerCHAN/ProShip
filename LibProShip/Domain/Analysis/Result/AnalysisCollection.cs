using System.Collections.Generic;

namespace LibProShip.Domain.Analysis.Result
{
    public interface IAnalysisResult
    {
        
    }

    
    public sealed class AnalysisCollection : ValueObject
    {
        
        
        public AnalysisCollection(IDictionary<string, string> properties, SphereChartResult sphereChart)
        {
            this.Properties = properties;
            this.SphereCharts = sphereChart;
        }

        public AnalysisCollection()
        {
            //For ORM
        }

        public IDictionary<string, string> Properties { get; private set; }
        public SphereChartResult SphereCharts { get; private set; }
    }
}