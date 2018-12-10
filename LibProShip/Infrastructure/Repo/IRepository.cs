using System.Collections.Generic;
using LibProShip.Domain;

namespace LibProShip.Infrastructure.Repo
{
    public interface IRepository<T> where T: Entity<T>
    {
        void Insert(T item);

        void Update(T item);

        void Remove(T item);

        IEnumerable<T> GetAll();
    }
}