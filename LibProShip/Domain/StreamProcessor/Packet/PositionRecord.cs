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
            Name = name;
            Id = id;
            ShipId = shipId;
        }

        public string Name { get; }
        public int Id { get; }
        public int ShipId { get; }
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


    public enum ProjectileType
    {
        Unknown = 0,
        Gun = 1,
        Torpedol = 2
    }

    public class ProjectileRecord
    {
        public ProjectileType ProjectileType { get; }
        public Player SourcePlayer { get; }
        public float ShootTime { get; }
        public Player TargetPlayer { get; }
        public float DentonateTime { get; }
    }
}