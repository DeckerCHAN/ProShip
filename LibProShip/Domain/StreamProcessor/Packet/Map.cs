using System.Collections.Generic;

namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Map
    {
        public Map(int id, string name, long arenaId, double distanceConvertRatio, int spaceId)
        {
            this.ArenaId = arenaId;
            this.SpaceId = spaceId;
            this.Id = id;
            this.DistanceConvertRatio = distanceConvertRatio;
            this.Name = name;
        }

        public string Name { get; }
        public int Id { get; }
        public long ArenaId { get; }
        public int SpaceId { get; }
        public double DistanceConvertRatio { get; }
    }
}