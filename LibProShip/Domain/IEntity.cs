using System;

namespace LibProShip.Domain
{
    public interface IEntity<T>
    {
        Guid Identity { get; }
    }
}