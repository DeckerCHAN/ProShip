using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Events
{
    public class AnalysisUpdatedEvent : IDomainEvent
    {
        public AnalysisUpdatedEvent(object source, string replayId)
        {
            this.Source = source;
            this.ReplayId = replayId;
        }

        public string ReplayId { get; }
        public object Source { get; }
    }
}