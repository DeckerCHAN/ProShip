using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Decode.Event
{
    public class NewRawReplayEvent : IDomainEvent
    {
        public NewRawReplayEvent(object source, RawReplay rawReplay)
        {
            this.RawReplay = rawReplay;
            this.Source = source;
        }

        public RawReplay RawReplay { get; }

        public object Source { get; }
    }
}