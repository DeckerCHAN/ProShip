namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Map
    {
        public Map(int id, string name, long arenaId, double distanceConvertRatio, int spaceId)
        {
            ArenaId = arenaId;
            SpaceId = spaceId;
            Id = id;
            DistanceConvertRatio = distanceConvertRatio;
            Name = name;
        }

        public string Name { get; }
        public int Id { get; }
        public long ArenaId { get; }
        public int SpaceId { get; }
        public double DistanceConvertRatio { get; }
    }
}