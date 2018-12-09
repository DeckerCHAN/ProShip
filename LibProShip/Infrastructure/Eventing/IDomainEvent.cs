namespace LibProShip.Infrastructure.Eventing
{
    public interface IDomainEvent
    {
        object Source { get; }
    }
}