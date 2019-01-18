using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibProShip.Infrastructure.Eventing
{
    public sealed class DefaultEventBus : IEventBus
    {
        public DefaultEventBus()
        {
            Handlers = new List<Tuple<Type, object>>();
        }

        public ICollection<Tuple<Type, object>> Handlers { get; }


        public void Unregister(IDomainEventHandler<IDomainEvent> handler)
        {
            var foundHandlers = Handlers.Where(x => x.Item2 == handler);
            foreach (var foundHandler in foundHandlers) Handlers.Remove(foundHandler);
        }

        public void Unregister(IDomainEvent domainEventToUnregister)
        {
            throw new NotImplementedException();
        }


        public void Register<T>(IDomainEventHandler<T> handler) where T : IDomainEvent
        {
            Handlers.Add(new Tuple<Type, dynamic>(typeof(T), handler));
        }

        public async void Raise<T>(T ie) where T : IDomainEvent
        {
            var handlers = Handlers.Where(x => ie.GetType() == x.Item1).Select(x => x.Item2).ToList();
            foreach (var eHandler in handlers)
                if (eHandler is IDomainEventHandler<T>)
                {
                    var handler = (IDomainEventHandler<T>) eHandler;
                    await Task.Run(() => { handler.Handle(ie); });
                }
        }
    }
}