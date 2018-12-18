using System;
using LibProShip.Domain.Events;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Application
{
    public class DomainEventPublisher : IDomainEventHandler<IDomainEvent>
    {
        private readonly IEventBus EventBus;

        public event EventHandler NewReplaySaved;

        public DomainEventPublisher(IEventBus eventBus)
        {
            this.EventBus = eventBus;
        }

        public void Handle(IDomainEvent e)
        {
            if (e.GetType() == typeof(NewReplaySavedEvent))
            {
                var handler = this.NewReplaySaved;
                handler?.Invoke(this, new EventArgs());
            }
            else
            {
            }
        }

        public void Init()
        {
            this.EventBus.Register(this);
        }
    }
}