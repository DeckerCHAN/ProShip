using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using LiteDB;

namespace LibProShip.Domain.Replay
{
    public class ReplayRepository : IDisposable
    {
        public ReplayRepository()
        {
            var fileName = "RawReplay4.ldb";
            Context = new LiteDatabase(fileName);
            Collection = Context.GetCollection<Replay>();
            FileStorage = Context.FileStorage;
        }

        private LiteCollection<Replay> Collection { get; }

        private LiteStorage FileStorage { get; }

        private LiteDatabase Context { get; }

        public void Dispose()
        {
            Context?.Dispose();
        }


        public void Insert(Replay replay, byte[] replayFile)
        {
            Collection.Insert(replay);
            FileStorage.Upload(replay.Id, replay.FileName, new MemoryStream(replayFile));
        }

        public IEnumerable<Replay> Find(Expression<Func<Replay, bool>> predict)
        {
            return Collection.Find(predict);
        }

        public IEnumerable<Replay> Paging(int numberPrePage, int pageIndex)
        {
            return Collection.FindAll()
                .OrderByDescending(x => x.Battle.DateTime)
                .Skip(numberPrePage * pageIndex)
                .Take(numberPrePage);
        }

        public byte[] FindFile(Replay replays)
        {
            var found = FileStorage.FindById(replays.Id);
            if (found == null) return null;

            using (var stream = found.OpenRead())
            {
                var res = new byte[stream.Length];
                stream.Read(res, 0, res.Length);
                return res;
            }
        }

        public void Update(Replay replay)
        {
            this.Collection.Update(replay);
        }
    }
}