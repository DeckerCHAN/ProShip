using System;
using System.Collections.Generic;
using System.Xml;

namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Arena
    {
    }

    public sealed class Vehicle
    {
        public Vehicle(int vehicleId, Player controlPlayer)
        {
            ControlPlayer = controlPlayer;
            VehicleId = vehicleId;
        }

        public int VehicleId { get; }
        public Player ControlPlayer { get; }

        public override bool Equals(object obj)
        {
            return obj is Vehicle item && this.VehicleId.Equals(item.VehicleId);
        }

        public static bool operator ==(Vehicle vehicle1, Vehicle vehicle2)
        {
            return vehicle1?.Equals(vehicle2) ?? ReferenceEquals(vehicle2, null);
        }

        public static bool operator !=(Vehicle vehicle1, Vehicle vehicle2)
        {
            return !(vehicle1 == vehicle2);
        }

        public override int GetHashCode()
        {
            return this.VehicleId.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{this.VehicleId}]{this.ControlPlayer.Name}";
        }
    }

    public sealed class Player
    {
        public Player(string name, int accountId, long shipId)
        {
            this.Name = name;
            this.AccountId = accountId;
            this.ShipId = shipId;
        }

        public string Name { get; }
        public int AccountId { get; }
        public long ShipId { get; }

        public static bool operator ==(Player p1, Player p2)
        {
            return p1?.Equals(p2) ?? ReferenceEquals(p2, null);
        }

        public static bool operator !=(Player p1, Player p2)
        {
            return !(p1 == p2);
        }

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
        protected ProjectileShootRecord(Vehicle ownerVehicle, float shootTime, int shotId, int salvoId,
            Matrix3 position,
            Matrix3 direction)
        {
            this.OwnerVehicle = ownerVehicle;
            ShootTime = shootTime;
            ShotId = shotId;
            SalvoId = salvoId;
            Position = position;
            Direction = direction;
        }

        public Vehicle OwnerVehicle { get; }
        public float ShootTime { get; }
        public int ShotId { get; }
        public int SalvoId { get; }
        public Matrix3 Position { get; }
        public Matrix3 Direction { get; }
    }

    public class GunShootRecord : ProjectileShootRecord
    {
        public GunShootRecord(Vehicle ownerVehicle, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction, Matrix3 targetPosition, float hitDistance, int gunBarrelId) : base(ownerVehicle,
            shootTime,
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
        public TorpedoShootRecord(Vehicle ownerVehicle, float shootTime, int shotId, int salvoId, Matrix3 position,
            Matrix3 direction) : base(ownerVehicle, shootTime, shotId, salvoId, position, direction)
        {
        }
    }

    public sealed class HitRecord
    {
        public HitRecord(Vehicle ownerVehicle, float hitTime, Matrix3 position, int shotId, int hitType)
        {
            Position = position;
            this.OwnerVehicle = ownerVehicle;
            ShotId = shotId;
            HitType = hitType;
            HitTime = hitTime;
        }

        public float HitTime { get; }
        public Matrix3 Position { get; }
        public Vehicle OwnerVehicle { get; }
        public int ShotId { get; }
        public int HitType { get; }
    }

    public sealed class DamageRecord
    {
        public DamageRecord(float time, Vehicle sourceVehicle, Vehicle targetVehicle, float amount)
        {
            Time = time;
            SourceVehicle = sourceVehicle;
            TargetVehicle = targetVehicle;
            Amount = amount;
        }

        public float Time { get; }
        public Vehicle SourceVehicle { get; }
        public Vehicle TargetVehicle { get; }
        public float Amount { get; }
    }
}