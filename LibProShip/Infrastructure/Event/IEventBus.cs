namespace LibProShip.Infrastructure.Event
{
    public interface IEventBus
    {
        void Register<T>(IDomainEventHandler<T> handler) where T : IDomainEvent;
        void Raise<T>(T domainEvent) where T : IDomainEvent;
        void Unregister(IDomainEventHandler<IDomainEvent> handler);
        void Unregister(IDomainEvent domainEventToUnregister);
    }
}