using System;

namespace LibProShip.Domain
{
    public abstract class Entity<T>
    {
        protected Entity(Guid id)
        {
            this.Id = id;
        }

        public Entity()
        {
            
        }

        public Guid Id { get; set; }
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