using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Events
{
    public class NewReplaySavedEvent : IDomainEvent
    {
        public NewReplaySavedEvent(object source)
        {
            Source = source;
        }

        public object Source { get; }
    }
}