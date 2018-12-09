using System.Collections.Generic;
using System.Linq;
using LibProShip.Domain.Decode;
using LibProShip.Domain.Decode.Event;
using LibProShip.Domain.Parse.Events;
using LibProShip.Infrastructure.Eventing;
using LibProShip.Infrastructure.Repo;

namespace LibProShip.Domain.Parse
{
    public class NewRawReplayHandler : IDomainEventHandler<NewRawReplayEvent>
    {
        private readonly IEventBus EventBus;
        private readonly IEnumerable<IReplayProcessor> Processors;
        private readonly IRepository<Replay> Repository;


        public NewRawReplayHandler(IEventBus eventBus, IEnumerable<IReplayProcessor> processors)
        {
            this.EventBus = eventBus;
            this.Processors = processors;
        }

        public void Handle(NewRawReplayEvent e)
        {
            var rawReplay = e.RawReplay;
            var replay = this.Processors
                .Select(x => x.Process(rawReplay))
                .Where(x => x != null)
                .DefaultIfEmpty(null)
                .FirstOrDefault();
            this.Repository.Insert(replay);
            this.EventBus.Raise(new NewReplaySavedEvent(this));
        }

        public void Init()
        {
            this.EventBus.Register(this);
        }
    }
}