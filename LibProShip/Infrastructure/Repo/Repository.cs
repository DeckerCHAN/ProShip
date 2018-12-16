using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using LibProShip.Domain;
using LibProShip.Domain.Decode;
using LiteDB;

namespace LibProShip.Infrastructure.Repo
{
    public class Repository<T> : IDisposable, IRepository<T> where T : Entity<T>
    {
        public Repository()
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
            this.collection.Insert(item);
        }

        public void Update(T item)
        {
            this.collection.Update(item);
        }

        public void Remove(Expression<Func<T, bool>> predict)
        {
            throw new NotImplementedException();
        }

        public void Remove(T item)
        {
            this.collection.Delete(arg => Equals(arg, item));
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predict)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            return this.collection.FindAll();
        }
    }
}