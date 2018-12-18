using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Events
{
    public class NewRawReplayEvent : IDomainEvent
    {
        public NewRawReplayEvent(object source)
        {
            this.Source = source;
        }


        public object Source { get; }
    }
}