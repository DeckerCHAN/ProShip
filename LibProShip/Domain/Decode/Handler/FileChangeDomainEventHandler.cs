using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LibProShip.Domain.Decode.Event;
using LibProShip.Domain.Decode.Interface;
using LibProShip.Domain.FileSystem;
using LibProShip.Infrastructure.Event;

namespace LibProShip.Domain.Decode.Handler
{
    public class FileChangeDomainEventHandler : IDomainEventHandler<FileChangeDomainEvent>
    {
        private readonly IEnumerable<IDecoder> Decoders;
        private readonly IReplayAssembiler ReplayAssembler;
        private readonly IRepository<Replay> Repository;
        private readonly IEventBus EventBus;

        public FileChangeDomainEventHandler(IReplayAssembiler replayAssembler,
            IRepository<Replay> repository, IEventBus eventBus, IEnumerable<IDecoder> decoders)
        {
            this.ReplayAssembler = replayAssembler;
            this.Repository = repository;
            this.EventBus = eventBus;
            this.Decoders = decoders;
        }


        public void Handle(FileChangeDomainEvent e)
        {
            var files = e.ReplayFiles;

            foreach (var file in files)
            {
                var raw = this.Decoders.Select(x => x.DecodeReplay(file)).DefaultIfEmpty(null)
                    .FirstOrDefault(x => x != null);
                if (raw == null)
                {
                    //TODO: We need log here
                    return;
                }

                var replay = this.ReplayAssembler.Assembly(raw);
                this.Repository.Insert(replay);
                this.EventBus.Raise(new NewRawReplayEvent(this, raw));
            }
        }


        public void Init()
        {
            this.EventBus.Register(this);
        }
    }
}