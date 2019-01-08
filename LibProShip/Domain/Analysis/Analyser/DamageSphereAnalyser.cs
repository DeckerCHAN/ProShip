using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using LibProShip.Domain.Analysis.Dto;
using LibProShip.Domain.StreamProcessor.Packet;
using LibProShip.Domain.StreamProcessor.Packet.Extensions;

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

        private Vehicle GetDamageSource(IEnumerable<PositionRecord> positionRecords,IEnumerable<HitRecord> hitRecords, IEnumerable<DamageRecord> damageRecords,
            DamageRecord record)
        {
            var hitTime = record.Time;
            var victim = record.TargetVehicle;
            var lastDamageRecordTime = damageRecords
                .Where(x => x.Time < hitTime)
                .Select(x => x.Time)
                .OrderByDescending(x => x)
                .DefaultIfEmpty(0)
                .FirstOrDefault();

            var relativeHits = hitRecords
                .Where(x => x.OwnerVehicle == record.SourceVehicle)
                //TODO: Add pos restriction
                .Where(x => x.HitTime < hitTime && x.HitTime > lastDamageRecordTime && x.HitTime > hitTime - 5F)
                .Where(hit => GetVehiclePosition(positionRecords, hit.HitTime, victim).DistanceFrom(hit.Position) < 10F)
                .ToArray();

        }

        private Matrix3 GetVehiclePosition(IEnumerable<PositionRecord> positionRecords, float time, Vehicle vehicle)
        {
            var enumerable = positionRecords as PositionRecord[] ?? positionRecords.ToArray();
            var lowerNearest = enumerable
                .Where(x => x.Vehicle == vehicle)
                .Where(x => x.Time <= time)
                .OrderByDescending(x => x.Time)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            var higherNearest = enumerable
                .Where(x => x.Vehicle == vehicle)
                .Where(x => x.Time >= time)
                .OrderBy(x => x.Time)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (lowerNearest == null && higherNearest == null)
            {
                return null;
            }
            else if (lowerNearest == null)
            {
                return higherNearest.Position;
            }
            else if (higherNearest == null)
            {
                return lowerNearest.Position;
            }
            else
            {
                var x = (lowerNearest.Position.X + higherNearest.Position.X) / 2;
                var y = (lowerNearest.Position.Y + higherNearest.Position.Y) / 2;
                var z = (lowerNearest.Position.Z + higherNearest.Position.Z) / 2;

                return new Matrix3(x, y, z);
            }
        }
    }
}