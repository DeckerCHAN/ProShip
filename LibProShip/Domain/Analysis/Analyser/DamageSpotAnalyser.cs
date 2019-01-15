using System;
using System.Collections.Generic;
using System.Linq;
using LibProShip.Domain.Analysis.Result;
using LibProShip.Domain.StreamProcessor.Packet;
using LibProShip.Domain.StreamProcessor.Packet.Extensions;

namespace LibProShip.Domain.Analysis.Analyser
{
    public class DamageSpotAnalyser : SphereAnalyserBase
    {
        public DamageSpotAnalyser()
        {
            Name = "Damage Spot Analyser";
        }

        public override string Name { get; }

        public override AnalysisCollection Analysis(BattleRecord battleRecord)
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

                var hits = this.GetRelatedHits(battleRecord.PositionRecords, battleRecord.HitRecords,
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

                var relativeAngleToVictim = this.GetRelativeRotationFromGunHit(victimPosition, sourceGun);

                var distanceFromVictim = sourceGun.Position.DistanceFrom(victimPosition.position);
                
                var spot = new SpotSample(string.Empty, damageRecord.Amount, Color.RED, distanceFromVictim * battleRecord.Map.DistanceConvertRatio,
                    relativeAngleToVictim);
                sp.Add(spot);
            }

            var proprites = new Dictionary<string, string>();
            proprites["Title"] = "Damage Spots";

            var col = new SphereChartResult(sp, new PointSample[0], "Damage Spot");
            return new AnalysisCollection(proprites, new[] {col});
        }
    }
}