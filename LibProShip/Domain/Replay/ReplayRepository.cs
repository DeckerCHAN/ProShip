using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using LibProShip.Domain.Decode;
using LiteDB;

namespace LibProShip.Domain2.Replay
{
    public class ReplayRepository : IDisposable
    {
        public ReplayRepository()
        {
            var fileName = $"RawReplay4.ldb";
            this.Context = new LiteDatabase(fileName);
            this.Collection = this.Context.GetCollection<Replay>();
            this.FileStorage = this.Context.FileStorage;
        }

        private LiteCollection<Replay> Collection { get; set; }

        private LiteStorage FileStorage { get; set; }

        private LiteDatabase Context { get; set; }


        public void Insert(Replay replay, byte[] replayFile)
        {
            this.Collection.Insert(replay);
            this.FileStorage.Upload(replay.Id, replay.FileName, new MemoryStream(replayFile));
        }

        public IEnumerable<Replay> Find(Expression<Func<Replay, bool>> predict)
        {
            return this.Collection.Find(predict);
        }

        public byte[] FindFile(Replay replays)
        {
            var found = this.FileStorage.FindById(replays.Id);
            if (found == null)
            {
                return null;
            }

            using (var stream  = found.OpenRead())
            {
                var res = new byte[stream.Length];
                stream.Read(res, 0, res.Length);
                return res;
            }
        }

        public void Update(Replay replay)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.Context?.Dispose();
        }
    }
}