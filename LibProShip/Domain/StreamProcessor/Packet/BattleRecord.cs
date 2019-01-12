using System.Collections.Generic;

namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class BattleRecord
    {
        public BattleRecord(long arenaId, Map map,
            Vehicle controlVehicle,
            IEnumerable<Player> players,
            IEnumerable<Vehicle> vehicles, IEnumerable<PositionRecord> positionRecords,
            IEnumerable<TorpedoShootRecord> torpedoShootRecords,
            IEnumerable<GunShootRecord> gunShootRecords, IEnumerable<HitRecord> hitRecords,
            IEnumerable<DamageRecord> damageRecords)
        {
            this.PositionRecords = positionRecords;
            this.Map = map;
            this.ControlVehicle = controlVehicle;
            this.Players = players;
            this.Vehicles = vehicles;
            this.ArenaId = arenaId;
            this.TorpedoShootRecords = torpedoShootRecords;
            this.GunShootRecords = gunShootRecords;
            this.HitRecords = hitRecords;
            this.DamageRecords = damageRecords;
        }

        public Map Map { get; }
        public Vehicle ControlVehicle { get; }
        public IEnumerable<Player> Players { get; }
        public IEnumerable<Vehicle> Vehicles { get; }
        public IEnumerable<PositionRecord> PositionRecords { get; }
        public long ArenaId { get; }
        public IEnumerable<TorpedoShootRecord> TorpedoShootRecords { get; }
        public IEnumerable<GunShootRecord> GunShootRecords { get; }
        public IEnumerable<HitRecord> HitRecords { get; }
        public IEnumerable<DamageRecord> DamageRecords { get; }
    }
}