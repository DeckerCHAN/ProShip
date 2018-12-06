namespace LibProShip.Infrastructure.Event
{
    public interface IDomainEvent
    {
        object Source { get; }
    }
}