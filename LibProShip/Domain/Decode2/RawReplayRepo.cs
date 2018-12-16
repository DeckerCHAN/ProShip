using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using LibProShip.Domain.Decode;
using LibProShip.Infrastructure.Repo;
using LiteDB;

namespace LibProShip.Domain.Decode2
{
    public sealed class RawReplayRepo : IRepository<RawReplay>
    {
        private LiteStorage FileStorage { get; set; }

        public RawReplayRepo()
        {
            var fileName = $"RawReplay.ldb";
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }

            this.Context = new LiteDatabase(fileName);
            this.Collection = this.Context.GetCollection<RawReplay>();
            this.FileStorage = this.Context.FileStorage;
        }

        private LiteCollection<RawReplay> Collection { get; set; }

        private LiteDatabase Context { get; set; }

        public void Insert(RawReplay item)
        {
            var bytes = item.Data;
            var simpReplay = new RawReplay(item.Id, item.FileName, item.Battle, null);
            this.Collection.Insert(simpReplay);
            using (var zipedStream = new MemoryStream())
            {
                var streamToSave = new MemoryStream();
                this.Compress(bytes, streamToSave);
                this.FileStorage.Upload(simpReplay.Id.ToString(), simpReplay.FileName, zipedStream);
            }
        }

        public void Update(RawReplay item)
        {
            throw new NotImplementedException();
        }

        public void Remove(Expression<Func<RawReplay, bool>> predict)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RawReplay> GetAll(Expression<Func<RawReplay, bool>> predict)
        {
            var collection = this.Collection.Find(predict).ToArray();

            var resCollection = new LinkedList<RawReplay>();
            foreach (var rawReplay in collection)
            {
                var stream = this.FileStorage.FindById(rawReplay.Id.ToString()).OpenRead();
                var bytes = this.Decompress(stream);
                var newObj = new RawReplay(rawReplay.Id, rawReplay.FileName, rawReplay.Battle, bytes);
                resCollection.AddLast(newObj);
            }

            return resCollection;
        }


        private byte[] Decompress(Stream gzipedData)
        {
            using (var output = new MemoryStream())
            using (var def = new DeflateStream(gzipedData, CompressionMode.Decompress))
            {
                def.CopyTo(output);
                var data = new byte [output.Length];
                output.Read(data, 0, data.Length);
                return data;
            }
        }

        private void Compress(byte[] data, Stream output)
        {
            using (var originStream = new MemoryStream(data))
            using (var def = new DeflateStream(output, CompressionLevel.Optimal))
            {
                originStream.CopyTo(def);
            }
        }
    }
}