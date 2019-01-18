using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibProShip.Domain.Analysis.Result;
using LibProShip.Domain.Decoding;
using LibProShip.Domain.Events;
using LibProShip.Infrastructure.Eventing;
using LibProShip.Infrastructure.Logging;
using LibProShip.Infrastructure.Utils;

namespace LibProShip.Domain.Replay
{
    public class RawFileProcessor : IDomainEventHandler<FileChangeEvent>
    {
        private readonly IEnumerable<IDecoder> Decoders;
        private readonly IEventBus EventBus;
        private readonly ILogger Logger;
        private readonly ReplayRepository Repository;
        private readonly Queue<FileInfo> UnProcessedFilePool;

        public RawFileProcessor(ReplayRepository repository, IEventBus eventBus,
            IEnumerable<IDecoder> decoders,
            ILogger logger)
        {
            Repository = repository;
            EventBus = eventBus;
            Decoders = decoders;
            Logger = logger;
            UnProcessedFilePool = new Queue<FileInfo>();
        }


        public void Handle(FileChangeEvent e)
        {
            var files = e.ReplayFiles;

            lock (UnProcessedFilePool)
            {
                foreach (var file in files)
                {
                    if (Repository.Find(x => x.FileName.Equals(file.Name)).Any()) continue;

                    //TODO: Something went wrong here
                    UnProcessedFilePool.Enqueue(file);
                }
            }
        }


        public void Init()
        {
            EventBus.Register(this);
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(10000);
                    try
                    {
                        ProcessQueuedFile();
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            });
        }

        private void ProcessQueuedFile()
        {
            FileInfo repFile;
            lock (UnProcessedFilePool)
            {
                repFile = UnProcessedFilePool.Dequeue();
            }


            var decoded = Decoders.Select(x =>
            {
                try
                {
                    return x.DecodeReplay(repFile);
                }
                catch (Exception e)
                {
                    return null;
                }
            }).DefaultIfEmpty(null).FirstOrDefault(x => x != null);

            if (decoded == null) return;
            Logger.Info($"{repFile.Name} Processed");
            var replay = new Replay(HashUtils.Hash(decoded.Item2), repFile.Name, decoded.Item1,
                new Dictionary<string, AnalysisCollection>());

            Repository.Insert(replay, decoded.Item2);
            EventBus.Raise(new NewRawReplayEvent(this));
        }
    }
}