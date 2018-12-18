using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibProShip.Domain;
using LibProShip.Domain.Decode.Event;
using LibProShip.Domain2.Analysis;
using LibProShip.Domain2.Events;
using LibProShip.Infrastructure.Eventing;
using LibProShip.Infrastructure.Logging;
using LibProShip.Infrastructure.Utils;

namespace LibProShip.Domain2.Replay
{
    public class RawFileProcessor : IDomainEventHandler<FileChangeEvent>
    {
        private readonly IEnumerable<Decoding.IDecoder> Decoders;
        private readonly ReplayRepository Repository;
        private readonly IEventBus EventBus;
        private readonly Queue<FileInfo> UnProcessedFilePool;
        private readonly ILogger Logger;

        public RawFileProcessor(ReplayRepository repository, IEventBus eventBus,
            IEnumerable<Decoding.IDecoder> decoders,
            ILogger logger)
        {
            this.Repository = repository;
            this.EventBus = eventBus;
            this.Decoders = decoders;
            this.Logger = logger;
            this.UnProcessedFilePool = new Queue<FileInfo>();
        }

        private void ProcessQueuedFile()
        {
            FileInfo repFile;
            lock (this.UnProcessedFilePool)
            {
                repFile = this.UnProcessedFilePool.Dequeue();
            }


            var decoded = this.Decoders.Select(x =>
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

            if (decoded != null)
            {
                this.Logger.Info($"{repFile.Name} Processed");
                var replay = new Replay(HashUtils.Hash(decoded.Item2), repFile.Name, decoded.Item1,
                    new Dictionary<string, AnalysisResult>());

                this.Repository.Insert(replay, decoded.Item2);
                this.EventBus.Raise(new NewRawReplayEvent(this));
            }
            else
            {
                lock (this.UnProcessedFilePool)
                {
                    this.UnProcessedFilePool.Enqueue(repFile);
                    this.Logger.Info($"Unable to process {repFile.Name} add back to the queue");
                }
            }
        }


        public void Handle(FileChangeEvent e)
        {
            var files = e.ReplayFiles;

            lock (this.UnProcessedFilePool)
            {
                foreach (var file in files)
                {
                    if (this.Repository.Find(x => x.FileName.Equals(file.Name)).Any())
                    {
                        //This file been processed already
                        continue;
                    }

                    //TODO: Something went wrong here
                    this.UnProcessedFilePool.Enqueue(file);
                }
            }
        }


        public void Init()
        {
            this.EventBus.Register(this);
            Task.Run((async () =>
            {
                while (true)
                {
                    await Task.Delay(10000);
                    this.ProcessQueuedFile();
                }
            }));
        }
    }
}