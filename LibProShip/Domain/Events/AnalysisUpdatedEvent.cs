using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Events
{
    public class AnalysisUpdatedEvent : IDomainEvent
    {
        public AnalysisUpdatedEvent(object source, string replayId)
        {
            Source = source;
            ReplayId = replayId;
        }

        public string ReplayId { get; }
        public object Source { get; }
    }
}