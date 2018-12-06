using System;
using System.Collections.Generic;
using LiteDB;

namespace LibProShip.Domain
{
    public class Repository<T> : IDisposable
    {
        public Repository()
        {
            this.context = new LiteDatabase($"{typeof(T).Name}.ldb");
        }

        public void Insert()
        {
            var a = new Dictionary<string, string>();
            a.ContainsKey()
            
            this.context.GetCollection<T>()
        }

        public LiteDatabase context { get; }


        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}