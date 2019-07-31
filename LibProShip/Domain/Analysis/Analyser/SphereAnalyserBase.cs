using System;
using System.Collections.Generic;
using System.Linq;
using LibProShip.Domain.Analysis.Result;
using LibProShip.Domain.StreamProcessor.Packet;
using LibProShip.Domain.StreamProcessor.Packet.Extensions;
using LibProShip.Infrastructure.Utils;

namespace LibProShip.Domain.Analysis.Analyser
{
    public abstract class SphereAnalyserBase : IAnalyser
    {
        public abstract string Name { get; }
        public abstract AnalysisCollection Analysis(BattleRecord battleRecord);


        protected virtual double GetRelativeRotationFromGunHit((Matrix3 position, Matrix3 rotation) victimPosition,
            GunShootRecord shootRecord)
        {
            var absluteAngleToVictim = MathUtils.AngleFrom2D(
                victimPosition.position.X,
                victimPosition.position.Z,
                shootRecord.Position.X,
                shootRecord.Position.Z
            );

            var relativeAngleToVictim = absluteAngleToVictim - victimPosition.rotation.X;

            if (relativeAngleToVictim > Math.PI) relativeAngleToVictim = -2 * Math.PI + relativeAngleToVictim;

            if (relativeAngleToVictim < -Math.PI) relativeAngleToVictim = 2 * Math.PI - relativeAngleToVictim;

            return relativeAngleToVictim;
        }


        protected IEnumerable<HitRecord> GetRelatedHits(IEnumerable<PositionRecord> positionRecords,
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
                    GetVehiclePosition(positionRecords, hit.HitTime, victim)?.position.DistanceFrom(hit.HitPosition) <
                    10F)
                .ToArray();

            return relativeHits;
        }

        protected virtual (Matrix3 position, Matrix3 rotation)? GetVehiclePosition(
            IEnumerable<PositionRecord> positionRecords,
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

            if (lowerNearest == null && higherNearest == null) return null;

            if (lowerNearest == null) return (higherNearest.Position, higherNearest.Rotation);

            if (higherNearest == null) return (lowerNearest.Position, lowerNearest.Rotation);

            if (lowerNearest == higherNearest)
            {
                return (lowerNearest.Position, lowerNearest.Rotation);
            }

            var px = (lowerNearest.Position.X + higherNearest.Position.X) / 2;
            var py = (lowerNearest.Position.Y + higherNearest.Position.Y) / 2;
            var pz = (lowerNearest.Position.Z + higherNearest.Position.Z) / 2;

            var rx = MathUtils.MeanAngle(lowerNearest.Rotation.X, higherNearest.Rotation.X);
            var ry = MathUtils.MeanAngle(lowerNearest.Rotation.Y, higherNearest.Rotation.Y);
            var rz = MathUtils.MeanAngle(lowerNearest.Rotation.Z, higherNearest.Rotation.Z);


            return (new Matrix3(px, py, pz), new Matrix3(rx, ry, rz));
        }
    }
}