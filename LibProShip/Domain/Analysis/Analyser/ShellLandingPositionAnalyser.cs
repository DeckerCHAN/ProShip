using System;
using System.Collections.Generic;
using System.Linq;
using LibProShip.Domain.Analysis.Result;
using LibProShip.Domain.StreamProcessor.Packet;
using LibProShip.Domain.StreamProcessor.Packet.Extensions;
using LibProShip.Infrastructure.Utils;

namespace LibProShip.Domain.Analysis.Analyser
{
    public sealed class ShellLandingPositionAnalyser : SphereAnalyserBase
    {
        private static double VehicleFindTimeLimitation = 5D;
        private static double VehicleFindDistanceLimitation = 200D;

        public ShellLandingPositionAnalyser()
        {
            Name = "Shell Landing Analysis";
        }

        public override string Name { get; }

        public override AnalysisCollection Analysis(BattleRecord battleRecord)
        {
            var allVehicles = battleRecord.EnemyVehicles.Concat(battleRecord.AliesVehicles).Distinct().ToArray();
            var results = new List<SphereChartResult>();
            foreach (var vehicle in allVehicles)
            {

                var gunShotIds = battleRecord.GunShootRecords
                    .Where(x => x.OwnerVehicle == vehicle)
                    .Select(x => x.ShotId);
                var gunHits = battleRecord.HitRecords
                    .Where(x => x.OwnerVehicle == vehicle)
                    .Where(x => gunShotIds.Contains(x.ShotId));

                //TODO: Continue plot points
                var points = gunHits.Select(gunHit =>
                {
                    var gunShot = battleRecord.GunShootRecords.First(x => x.ShotId == gunHit.ShotId);
                    var inRangeVehicles = allVehicles
                        .Where(x => x != vehicle) //There is no po
                        .Where(x =>
                        {
                            var pos = this.GetVehiclePosition(battleRecord.PositionRecords, gunHit.HitTime, x);
                            if (!pos.HasValue)
                            {
                                return false;
                            }

                            return pos.Value.position.DistanceFrom(gunHit.HitPosition) *
                                   battleRecord.Map.DistanceConvertRatio <
                                   VehicleFindDistanceLimitation;
                        }).ToArray();

                    //No vehicle found
                    if (!inRangeVehicles.Any())
                    {
                        return null;
                    }

                    //Get cloest vehicle
                    var closetVehicle = inRangeVehicles.Count() == 1
                        ? inRangeVehicles.First()
                        : inRangeVehicles.OrderByDescending(x =>
                            this.GetVehiclePosition(battleRecord.PositionRecords, gunHit.HitTime, x).Value.position
                                .DistanceFrom(gunHit.HitPosition)).First();

                    var victimPosition =
                        this.GetVehiclePosition(battleRecord.PositionRecords, gunHit.HitTime, closetVehicle);


                    var relativeRotation = this.GetRelativeRotationFromGunHit(victimPosition.Value, gunShot);
                    var actualDistanceFromGunHitAndVictimVehicle =
                        gunHit.HitPosition.DistanceFrom(victimPosition.Value.position) *
                        battleRecord.Map.DistanceConvertRatio;

                    switch (gunHit.HitType)
                    {
                        case HitType.OutOfRange:
                            throw new Exception("Gun shot never out of range.");
                        case HitType.Miss:
                            return new PointSample($"{closetVehicle.ControlPlayer.Name} Miss", relativeRotation,
                                actualDistanceFromGunHitAndVictimVehicle, Color.BLUE);
                        case HitType.HitOnTheMountain:
                            //Man this is too effing rare
                            return new PointSample($"{closetVehicle.ControlPlayer.Name} Mount", relativeRotation,
                                actualDistanceFromGunHitAndVictimVehicle, Color.GREEN);
                        case HitType.Hit:
                            return new PointSample($"{closetVehicle.ControlPlayer.Name} Hit", relativeRotation,
                                actualDistanceFromGunHitAndVictimVehicle, Color.RED);
                        default:
                            throw new ArgumentOutOfRangeException($"Unknown {gunHit.HitType}.");
                    }
                }).Where(x => x!= null).AsParallel().ToArray();
                
                

                results.Add(new SphereChartResult(new List<SpotSample>(), points, vehicle.ControlPlayer.Name));
            }

            var col = new AnalysisCollection(new Dictionary<string, string>() {{"Name", this.Name}}, results);
            return col;
        }

        protected override (Matrix3 position, Matrix3 rotation)? GetVehiclePosition(
            IEnumerable<PositionRecord> positionRecords, float time, Vehicle vehicle)
        {
            var vehiclePositions = positionRecords
                .Where(x => x.Vehicle == vehicle)
                .ToList();

            var lowerNearest = vehiclePositions
                .Where(x => x.Time <= time)
                .Where(x => Math.Abs(x.Time - time) < VehicleFindTimeLimitation)
                .OrderByDescending(x => x.Time)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            var higherNearest = vehiclePositions
                .Where(x => x.Time >= time)
                .Where(x => Math.Abs(x.Time - time) < VehicleFindTimeLimitation)
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

                var rx = MathUtils.MeanAngle(lowerNearest.Rotation.X, higherNearest.Rotation.X);
                var ry = MathUtils.MeanAngle(lowerNearest.Rotation.Y, higherNearest.Rotation.Y);
                var rz = MathUtils.MeanAngle(lowerNearest.Rotation.Z, higherNearest.Rotation.Z);


                return (new Matrix3(px, py, pz), new Matrix3(rx, ry, rz));
            }
        }
    }
}