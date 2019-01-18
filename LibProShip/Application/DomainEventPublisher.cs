using System;
using LibProShip.Domain.Events;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Application
{
    public class DomainEventPublisher : IDomainEventHandler<IDomainEvent>
    {
        private readonly IEventBus EventBus;

        public DomainEventPublisher(IEventBus eventBus)
        {
            EventBus = eventBus;
        }

        public void Handle(IDomainEvent e)
        {
            if (e.GetType() == typeof(NewReplaySavedEvent))
            {
                var handler = NewReplaySaved;
                handler?.Invoke(this, new EventArgs());
            }
        }

        public void Init()
        {
            EventBus.Register(this);
        }

        public event EventHandler NewReplaySaved;
    }
}