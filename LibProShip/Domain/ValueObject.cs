using System;

namespace LibProShip.Domain
{
    public abstract class ValueObject<T>
    {
        public long Id { get; protected set; }

        protected ValueObject()
        {
            
        }

        protected ValueObject(long id)
        {
            this.Id = id;
        }
    }
}