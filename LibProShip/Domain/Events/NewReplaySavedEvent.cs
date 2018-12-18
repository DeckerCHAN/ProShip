using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Events
{
    public class NewReplaySavedEvent : IDomainEvent
    {
        public NewReplaySavedEvent(object source)
        {
            this.Source = source;
        }

        public object Source { get; }
    }
}