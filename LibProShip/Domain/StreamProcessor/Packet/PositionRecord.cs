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
        protected ProjectileShootRecord(Player owner, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction)
        {
            Owner = owner;
            ShootTime = shootTime;
            ShotId = shotId;
            SalvoId = salvoId;
            Position = position;
            Direction = direction;
        }

        public Player Owner { get; }
        public float ShootTime { get; }
        public int ShotId { get; }
        public int SalvoId { get; }
        public Matrix3 Position { get; }
        public Matrix3 Direction { get; }
    }

    public class GunShootRecord : ProjectileShootRecord
    {
        public GunShootRecord(Player owner, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction, Matrix3 targetPosition, float hitDistance, int gunBarrelId) : base(owner, shootTime,
            shotId, salvoId, position, direction)
        {
            TargetPosition = targetPosition;
            HitDistance = hitDistance;
            GunBarrelId = gunBarrelId;
        }

        public Matrix3 TargetPosition { get; }
        public float HitDistance { get; }
        public int GunBarrelId { get; }
    }

    public class TorpedoShootRecord:ProjectileShootRecord
    {
        public TorpedoShootRecord(Player owner, float shootTime, int shotId, int salvoId, Matrix3 position, Matrix3 direction) : base(owner, shootTime, shotId, salvoId, position, direction)
        {
        }
    }

    public class HitRecord
    {
        public HitRecord(Player owner, float hitTime, Matrix3 position, int shotId, int hitType)
        {
            Position = position;
            Owner = owner;
            ShotId = shotId;
            HitType = hitType;
            HitTime = hitTime;
        }
        public float HitTime { get; }
        public Matrix3 Position { get; }

        public Player Owner { get; }
        public int ShotId { get; }
        public int HitType { get; }
    }
}