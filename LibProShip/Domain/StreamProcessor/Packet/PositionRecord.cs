using System.Collections.Generic;

namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Arena
    {
    }

    public sealed class Vehicle
    {
        public Vehicle(long vehicleId, Player controlPlayer)
        {
            ControlPlayer = controlPlayer;
            VehicleId = vehicleId;
        }

        public long VehicleId { get; }
        public Player ControlPlayer { get; }
    }

    public sealed class Player
    {
        public Player(string name, int accountId, int shipId)
        {
            this.Name = name;
            this.AccountId = accountId;
            this.ShipId = shipId;
        }

        public string Name { get; }
        public int AccountId { get; }
        public int ShipId { get; }

        public override bool Equals(object obj)
        {
            return obj is Player item && this.AccountId.Equals(item.AccountId);
        }

        public override int GetHashCode()
        {
            return this.AccountId.GetHashCode();
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

        public Map Map { get; }
        public IEnumerable<Player> Players { get; }
        public IEnumerable<Vehicle> Vehicles { get; }
        public IEnumerable<PositionRecord> PositionRecords { get; private set; }
    }

    public class PositionRecord
    {
        public PositionRecord(float time, Vehicle vehicle, Matrix3 position, Matrix3 rotation)
        {
            this.Position = position;
            this.Vehicle = vehicle;
            this.Time = time;
            this.Rotation = rotation;
        }

        public float Time { get; }
        public Vehicle Vehicle { get; }
        public Matrix3 Position { get; }
        public Matrix3 Rotation { get; }
    }


    public abstract class ProjectileShootRecord
    {
        protected ProjectileShootRecord(int ownerId, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction)
        {
            this.OwnerId = ownerId;
            ShootTime = shootTime;
            ShotId = shotId;
            SalvoId = salvoId;
            Position = position;
            Direction = direction;
        }

        public int OwnerId { get; }
        public float ShootTime { get; }
        public int ShotId { get; }
        public int SalvoId { get; }
        public Matrix3 Position { get; }
        public Matrix3 Direction { get; }
    }

    public class GunShootRecord : ProjectileShootRecord
    {
        public GunShootRecord(int ownerId, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction, Matrix3 targetPosition, float hitDistance, int gunBarrelId) : base(ownerId, shootTime,
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

    public class TorpedoShootRecord : ProjectileShootRecord
    {
        public TorpedoShootRecord(int ownerId, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction) : base(ownerId, shootTime, shotId, salvoId, position, direction)
        {
        }
    }

    public sealed class HitRecord
    {
        public HitRecord(int ownerId, float hitTime, Matrix3 position, int shotId, int hitType)
        {
            Position = position;
            this.OwnerId = ownerId;
            ShotId = shotId;
            HitType = hitType;
            HitTime = hitTime;
        }

        public float HitTime { get; }
        public Matrix3 Position { get; }
        public int OwnerId { get; }
        public int ShotId { get; }
        public int HitType { get; }
    }

    public sealed class DamageRecord
    {
        public DamageRecord(float time, Vehicle source, Vehicle target, float amount)
        {
            Time = time;
            Source = source;
            Target = target;
            Amount = amount;
        }

        public float Time { get; }
        public Vehicle Source { get; }
        public Vehicle Target { get; }
        public float Amount { get; }
    }
}