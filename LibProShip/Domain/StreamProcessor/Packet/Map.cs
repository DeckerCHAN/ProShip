namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Map
    {
        public Map(int arenaId, int spaceId)
        {
            this.ArenaId = arenaId;
            this.SpaceId = spaceId;
        }


        public int ArenaId { get; }
        public int SpaceId { get; }
    }
}