using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LibProShip.Infrastructure.Event;

namespace LibProShip.Application
{
    //TODO: Maybe useless?
    public class EventHandlerInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDomainEventHandler<IDomainEvent>>().LifestyleSingleton());
        }
    }
}