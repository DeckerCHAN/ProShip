using System.Collections.Generic;

namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class BattleRecord
    {
        public BattleRecord(long arenaId, Map map,
            Vehicle controlVehicle,
            IEnumerable<Player> players,
            IEnumerable<Vehicle> enemyVehicles, IEnumerable<Vehicle> aliesVehicles,
            IEnumerable<PositionRecord> positionRecords,
            IEnumerable<TorpedoShootRecord> torpedoShootRecords,
            IEnumerable<GunShootRecord> gunShootRecords, IEnumerable<HitRecord> hitRecords,
            IEnumerable<DamageRecord> damageRecords)
        {
            PositionRecords = positionRecords;
            Map = map;
            ControlVehicle = controlVehicle;
            Players = players;
            EnemyVehicles = enemyVehicles;
            ArenaId = arenaId;
            TorpedoShootRecords = torpedoShootRecords;
            GunShootRecords = gunShootRecords;
            HitRecords = hitRecords;
            DamageRecords = damageRecords;
            AliesVehicles = aliesVehicles;
        }

        public Map Map { get; }
        public Vehicle ControlVehicle { get; }
        public IEnumerable<Player> Players { get; }
        public IEnumerable<Vehicle> EnemyVehicles { get; }
        public IEnumerable<Vehicle> AliesVehicles { get; }
        public IEnumerable<PositionRecord> PositionRecords { get; }
        public long ArenaId { get; }
        public IEnumerable<TorpedoShootRecord> TorpedoShootRecords { get; }
        public IEnumerable<GunShootRecord> GunShootRecords { get; }
        public IEnumerable<HitRecord> HitRecords { get; }
        public IEnumerable<DamageRecord> DamageRecords { get; }
    }
}