using System.Collections.Generic;

namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Arena
    {
    }

    public sealed class Player
    {
        public Player(string name, int id, int shipId)
        {
            this.Name = name;
            this.Id = id;
            this.ShipId = shipId;
        }

        public string Name { get; }
        public int Id { get; }
        public int ShipId { get; }

        public override bool Equals(object obj)
        {
            return obj is Player item && this.Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }

    public sealed class BattleRecord
    {
        public BattleRecord(IEnumerable<PositionRecord> positionRecords, Map map, IEnumerable<Player> players)
        {
            this.PositionRecords = positionRecords;
            this.Map = map;
            this.Players = players;
        }

        public Map Map { get; private set; }
        public IEnumerable<Player> Players { get; private set; }
        public IEnumerable<PositionRecord> PositionRecords { get; private set; }
    }

    public class PositionRecord
    {
        public PositionRecord(float time, Player player, Matrix3 position, Matrix3 rotation)
        {
            this.Position = position;
            this.Player = player;
            this.Time = time;
            this.Rotation = rotation;
        }

        public float Time { get; }
        public Player Player { get; }
        public Matrix3 Position { get; }
        public Matrix3 Rotation { get; }
    }


    public abstract class ProjectileShootRecord
    {
        protected ProjectileShootRecord(Player owner, float shootTime)
        {
            this.Owner = owner;
            this.ShootTime = shootTime;
        }

        public Player Owner { get; }
        public float ShootTime { get; }
    }

    public class GunShootShootRecord: ProjectileShootRecord
    {
        public Matrix3 Position { get; }
        public Matrix3 Direction { get; }
        public Matrix3 TargetPosition { get; }
        public GunShootShootRecord(Player owner, float shootTime, Matrix3 position, Matrix3 direction, Matrix3 targetPosition) : base(owner, shootTime)
        {
            this.Position = position;
            this.Direction = direction;
            this.TargetPosition = targetPosition;
        }
    }
    
    
}