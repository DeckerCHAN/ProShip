namespace LibProShip.Domain2
{
    public abstract class Entity
    {
        public string Id { get; protected set; }


        protected Entity(string id)
        {
            this.Id = id;
        }

        protected Entity()
        {
        }
    }
}