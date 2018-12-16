using LibProShip.Domain;
using LibProShip.Domain2;

namespace LibProShip.Infrastructure.Eventing
{
    public interface IDomainEventHandler<in T>: IInit where T: IDomainEvent 
    {
        void Handle(T e);
    }
}