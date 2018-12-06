using LibProShip.Infrastructure.Event;

namespace LibProShip.Domain.Parse.Events
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