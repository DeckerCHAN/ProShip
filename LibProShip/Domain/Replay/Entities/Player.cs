namespace LibProShip.Domain.Replay.Entities
{
    public sealed class Player : ValueObject
    {
        public Player()
        {
        }

        public Player(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; }

        public long Id { get; }
    }
}