using System.Collections.Generic;
using System.Linq;
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

        private Vehicle GetDamageSource(IEnumerable<HitRecord> hitRecords,IEnumerable<DamageRecord> damageRecords,DamageRecord record)
        {
            var hitTime = record.Time;
            var lastDamageRecordTime = damageRecords
                .Where(x => x.Time < hitTime)
                .Select(x => x.Time)
                .OrderByDescending(x => x)
                .DefaultIfEmpty(0)
                .FirstOrDefault();
            
            var relativeHits = hitRecords
                .Where( x=> x.OwnerVehicle == record.SourceVehicle)
                //TODO: Add pos restriction
                .Where(x => x.HitTime < hitTime && x. HitTime > lastDamageRecordTime && x.HitTime> hitTime -5F )
            
        }
    }
}