namespace LibProShip.Domain
{
    public abstract class Entity
    {
        protected Entity(string id)
        {
            Id = id;
        }

        protected Entity()
        {
        }

        public string Id { get; protected set; }
    }
}