using System.Collections.Generic;

namespace LibProShip.Domain.Analysis.Dto
{
    public interface IAnalysisResult
    {
        
    }

    
    public sealed class AnalysisCollection : ValueObject
    {
        
        
        public AnalysisCollection(IDictionary<string, string> properties, ICollection<SphereChartResult> sphereCharts)
        {
            this.Properties = properties;
            this.SphereCharts = sphereCharts;
        }

        public AnalysisCollection()
        {
            //For ORM
        }

        public IDictionary<string, string> Properties { get; private set; }
        public ICollection<SphereChartResult> SphereCharts { get; private set; }
    }
}