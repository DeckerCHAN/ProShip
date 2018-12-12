using System;

namespace LibProShip.Domain
{
    public abstract class Entity<T>
    {
        protected Entity(Guid id)
        {
            this.Id = id;
        }

        protected Entity()
        {
            this.Id = Guid.Empty;
        }

        public Guid Id { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is Entity<T> item && this.Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}