using LibProShip.Domain.Analysis.Dto;
using LibProShip.Domain.StreamProcessor.Packet;

namespace LibProShip.Domain.Analysis.Analyser
{
    public class DamageSphereAnalyser : IAnalyser
    {
        public DamageSphereAnalyser()
        {
            this.Name = "Damage Sphere Analyser";
        }

        public string Name { get; }

        public AnalysisCollection Analysis(BattleRecord battleRecord)
        {
            throw new System.NotImplementedException();
        }
    }
}