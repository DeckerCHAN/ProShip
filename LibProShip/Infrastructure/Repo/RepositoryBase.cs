using System;
using System.Collections.Generic;
using System.IO;
using LibProShip.Domain.Decode;
using LiteDB;

namespace LibProShip.Infrastructure.Repo
{
    public  class RepositoryBase<T> : IDisposable
    {
        public RepositoryBase()
        {
            var fileName = $"{typeof(T).Name}.ldb";
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }

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
            throw new NotImplementedException();
        }

        public void Update(T item)
        {
            throw new NotImplementedException();
        }

        public void Remote(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            return this.collection.FindAll();
        }
    }
}