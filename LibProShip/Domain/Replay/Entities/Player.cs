namespace LibProShip.Domain.Replay.Entities
{
    public sealed class Player : ValueObject
    {
        public string Name { get; private set; }

        public Player()
        {
        }

        public Player(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public long Id { get; private set; }
    }
}