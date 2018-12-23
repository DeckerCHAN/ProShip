using System.Collections.Generic;

namespace LibProShip.Domain.Analysis.Dto
{
    public sealed class AnalysisResult : ValueObject
    {
        public AnalysisResult(IDictionary<string, string> properties, ICollection<SphereChart> sphereCharts)
        {
            this.Properties = properties;
            this.SphereCharts = sphereCharts;
        }

        public AnalysisResult()
        {
            //For ORM
        }

        public IDictionary<string, string> Properties { get; private set; }
        public ICollection<SphereChart> SphereCharts { get; private set; }
    }
}