using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LibProShip.Domain;

namespace LibProShip.Infrastructure.Repo
{
    public interface IRepository<T> where T: Entity<T>
    {
        void Insert(T item);

        void Update(T item);

        void Remove(Expression<Func<T, bool>> predict);

        IEnumerable<T> GetAll(Expression<Func<T, bool>> predict);
    }
}