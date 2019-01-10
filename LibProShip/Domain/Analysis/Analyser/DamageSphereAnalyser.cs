using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using LibProShip.Domain.Analysis.Result;
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


            var max = battleRecord.PositionRecords.Max(x => x.Rotation.X); 
            var min = battleRecord.PositionRecords.Min(x => x.Rotation.X); 
                
                
            double degreesM = (180 / Math.PI) * max;
            double degreesI = (180 / Math.PI) * min;

            
            
            var sp = new List<SpotSample>();
            foreach (var damageRecord in battleRecord.DamageRecords)
            {
                
                
                
                var hits =  this.GetRelativeHits(battleRecord.PositionRecords, battleRecord.HitRecords, battleRecord.DamageRecords,
                    damageRecord);
                
                //TODO: Here detect if same damage comes from two different type of projectile
                var hit = hits.FirstOrDefault();
                if (hit == null)
                {
                   continue;
                }

                var sourceGun = battleRecord.GunShootRecords
                    .Where(x => x.OwnerVehicle == hit.OwnerVehicle).FirstOrDefault(x => x.ShotId == hit.ShotId);
                

                if (sourceGun == null)
                {
                    continue;
                }
                
            }
            
            throw new System.NotImplementedException();
        }

        private IEnumerable<HitRecord> GetRelativeHits(IEnumerable<PositionRecord> positionRecords,IEnumerable<HitRecord> hitRecords, IEnumerable<DamageRecord> damageRecords,
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
                .Where(x => x.HitTime < hitTime && x.HitTime > lastDamageRecordTime && x.HitTime > hitTime - 5F)
                .Where(hit => this.GetVehiclePosition(positionRecords, hit.HitTime, victim)?.position.DistanceFrom(hit.Position) < 10F)
                .ToArray();

            return relativeHits;

        }

        private (Matrix3 position, Matrix3 rotation)? GetVehiclePosition(IEnumerable<PositionRecord> positionRecords, float time, Vehicle vehicle)
        {
            var vehiclePositions = positionRecords
                .Where(x => x.Vehicle == vehicle)
                .ToList();
            
            var lowerNearest = vehiclePositions
                .Where(x => x.Time <= time)
                .OrderByDescending(x => x.Time)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            var higherNearest = vehiclePositions
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
                return (higherNearest.Position,higherNearest.Rotation);
            }
            else if (higherNearest == null)
            {
                return (lowerNearest.Position,lowerNearest.Rotation);
            }
            else
            {
                var px = (lowerNearest.Position.X + higherNearest.Position.X) / 2;
                var py = (lowerNearest.Position.Y + higherNearest.Position.Y) / 2;
                var pz = (lowerNearest.Position.Z + higherNearest.Position.Z) / 2;
                
                var rx = (lowerNearest.Rotation.X + higherNearest.Rotation.X) / 2;
                var ry = (lowerNearest.Rotation.Y + higherNearest.Rotation.Y) / 2;
                var rz = (lowerNearest.Rotation.Z + higherNearest.Rotation.Z) / 2;
                
                
                return (new Matrix3(px, py, pz),new Matrix3(rx, ry, rz));
            }
        }
    }
}