using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LibProShip.Domain;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Application
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IInit, IDomainEventHandler<IDomainEvent>,DomainEventPublisher>().ImplementedBy<DomainEventPublisher>().LifestyleSingleton());
        }
    }
}