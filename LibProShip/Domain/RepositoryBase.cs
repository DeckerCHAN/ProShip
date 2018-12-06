using System;
using System.Collections.Generic;
using LiteDB;

namespace LibProShip.Domain
{
    public interface IRepository<T>
    {
        void Insert(T item);

        void Update(T item);

        void Remote(T item);

        IEnumerable<T> GetAll();
    }


    public abstract class RepositoryBase<T> : IDisposable
    {
        protected RepositoryBase()
        {
            this.context = new LiteDatabase($"{typeof(T).Name}.ldb");
            this.collection = this.context.GetCollection<T>();
        }

        private LiteDatabase context { get; }
        private LiteCollection<T> collection { get; }

        public void Dispose()
        {
            this.context.Dispose();
        }

        public void Insert(T item)
        {
        }

        public void Update(T item)
        {
        }

        public void Remote(T item)
        {
        }

        public IEnumerable<T> GetAll()
        {
            return this.collection.FindAll();
        }
    }
}