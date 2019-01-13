using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using LibProShip.Domain.Analysis.Result;
using LibProShip.Domain.StreamProcessor.Packet;
using LibProShip.Domain.StreamProcessor.Packet.Extensions;
using LibProShip.Infrastructure.Utils;

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
            var sp = new List<SpotSample>();
            foreach (var damageRecord in battleRecord.DamageRecords)
            {
                if (damageRecord.TargetVehicle != battleRecord.ControlVehicle)
                {
                    continue;
                }

                if (Math.Abs(damageRecord.Amount) < 1)
                {
                    continue;
                }

                var hits = this.GetRelativeHits(battleRecord.PositionRecords, battleRecord.HitRecords,
                    battleRecord.DamageRecords,
                    damageRecord).Where(x => x.HitType == HitType.Hit);

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

                var victimVehicle = damageRecord.TargetVehicle;
                var sourceVehicle = sourceGun.OwnerVehicle;

                var victimPosition = this.GetVehiclePosition(battleRecord.PositionRecords, hit.HitTime, victimVehicle)
                    .Value;
                var sourcePosition =
                    this.GetVehiclePosition(battleRecord.PositionRecords, sourceGun.ShootTime, sourceVehicle).Value;


                var absluteAngleToVictim = MathUtils.AngleFrom(
                    victimPosition.position.X,
                    victimPosition.position.Z,
                    sourcePosition.position.X,
                    sourcePosition.position.Z
                );

                var relativeAngleToVictim = absluteAngleToVictim - victimPosition.rotation.X;

                if (relativeAngleToVictim > Math.PI)
                {
                    relativeAngleToVictim = -2 * Math.PI + relativeAngleToVictim;
                }

                if (relativeAngleToVictim < (-Math.PI))
                {
                    relativeAngleToVictim = 2 * Math.PI - relativeAngleToVictim;
                }
                
        

                var distanceFromVictim = sourcePosition.position.DistanceFrom(victimPosition.position);

                var spot = new SpotSample(String.Empty, damageRecord.Amount, Color.RED, distanceFromVictim,
                    relativeAngleToVictim);
                sp.Add(spot);
            }

            var proprites = new Dictionary<string, string>();
            proprites["Title"] = "Damage Spots";

            var col = new SphereChartResult(sp, new PointSample[0]);
            return new AnalysisCollection(proprites, col);
        }

        private IEnumerable<HitRecord> GetRelativeHits(IEnumerable<PositionRecord> positionRecords,
            IEnumerable<HitRecord> hitRecords, IEnumerable<DamageRecord> damageRecords,
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
                .Where(hit =>
                    this.GetVehiclePosition(positionRecords, hit.HitTime, victim)?.position.DistanceFrom(hit.Position) <
                    10F)
                .ToArray();

            return relativeHits;
        }

        private (Matrix3 position, Matrix3 rotation)? GetVehiclePosition(IEnumerable<PositionRecord> positionRecords,
            float time, Vehicle vehicle)
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
                return (higherNearest.Position, higherNearest.Rotation);
            }
            else if (higherNearest == null)
            {
                return (lowerNearest.Position, lowerNearest.Rotation);
            }
            else if (lowerNearest == higherNearest)
            {
                return (lowerNearest.Position, lowerNearest.Rotation);
            }
            else
            {
                var px = (lowerNearest.Position.X + higherNearest.Position.X) / 2;
                var py = (lowerNearest.Position.Y + higherNearest.Position.Y) / 2;
                var pz = (lowerNearest.Position.Z + higherNearest.Position.Z) / 2;

                var rx = MeanAngle(lowerNearest.Rotation.X, higherNearest.Rotation.X);
                var ry = MeanAngle(lowerNearest.Rotation.Y, higherNearest.Rotation.Y);
                var rz = MeanAngle(lowerNearest.Rotation.Z, higherNearest.Rotation.Z);


                return (new Matrix3(px, py, pz), new Matrix3(rx, ry, rz));
            }
        }

        private static double MeanAngle(double r1, double r2)
        {
            var x = (Math.Cos(r1) + Math.Cos(r2)) / 2;
            var y = (Math.Sin(r1) + Math.Sin(r2)) / 2;
            return Math.Atan2(y, x);
        }
    }
}