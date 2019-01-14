using System.Collections.Generic;

namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Map
    {
        public Map(int arenaId, int spaceId, double distanceConvertRatio)
        {
            this.ArenaId = arenaId;
            this.SpaceId = spaceId;
            DistanceConvertRatio = distanceConvertRatio;
        }

        public int ArenaId { get; }
        public int SpaceId { get; }
        public double DistanceConvertRatio { get; }
    }
}