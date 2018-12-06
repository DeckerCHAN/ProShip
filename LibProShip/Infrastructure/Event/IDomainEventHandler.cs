using LibProShip.Domain;

namespace LibProShip.Infrastructure.Event
{
    public interface IDomainEventHandler<in T>: IInit where T: IDomainEvent 
    {
        void Handle(T e);
    }
}