using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibProShip.Domain.Decode.Event;
using LibProShip.Domain.FileSystem;
using LibProShip.Infrastructure.Eventing;
using LibProShip.Infrastructure.Logging;
using LibProShip.Infrastructure.Repo;

namespace LibProShip.Domain.Decode.Handler
{
    public class RawFileProcessor : IDomainEventHandler<FileChangeEvent>
    {
        private readonly IEnumerable<IDecoder> Decoders;
        private readonly IRepository<Replay> Repository;
        private readonly IEventBus EventBus;
        private readonly Queue<FileInfo> UnProcessedFilePool;
        private readonly ILogger Logger;

        public RawFileProcessor(IRepository<Replay> repository, IEventBus eventBus, IEnumerable<IDecoder> decoders,
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


            var raw = this.Decoders.Select(x =>
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

            if (raw != null)
            {
                this.Logger.Info($"{repFile.Name} Processed");
                this.EventBus.Raise(new NewRawReplayEvent(this, raw));
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