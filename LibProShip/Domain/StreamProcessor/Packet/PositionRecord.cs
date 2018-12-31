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
        public PositionRecord(Matrix3 matrix3, Player player, float time)
        {
            this.Matrix3 = matrix3;
            this.Player = player;
            this.Time = time;
        }

        public float Time { get; }
        public Player Player { get; }
        public Matrix3 Matrix3 { get; }
        
    }


    public class ProjectilePacket
    {
    }
}