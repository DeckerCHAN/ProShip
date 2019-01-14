using System.Linq;
using LibProShip.Domain.Analysis.Result;
using LibProShip.Domain.StreamProcessor.Packet;

namespace LibProShip.Domain.Analysis.Analyser
{
    public class ShellLandingPositionAnalyser : SphereAnalyserBase
    {
        public ShellLandingPositionAnalyser()
        {
            Name = "Shell Landing Analysis";
        }

        public override string Name { get; }

        public override AnalysisCollection Analysis(BattleRecord battleRecord)
        {
            foreach (var vehicle in battleRecord.Vehicles)
            {
                var hits = battleRecord.GunShootRecords.Where(x => x.OwnerVehicle == vehicle);
                //TODO: Continue plot points

            }
            

            throw new System.NotImplementedException();
        }
    }
}