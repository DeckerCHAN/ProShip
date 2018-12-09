using System.Collections.Generic;

namespace LibProShip.Infrastructure.Repo
{
    public interface IRepository<T>
    {
        void Insert(T item);

        void Update(T item);

        void Remote(T item);

        IEnumerable<T> GetAll();
    }
}