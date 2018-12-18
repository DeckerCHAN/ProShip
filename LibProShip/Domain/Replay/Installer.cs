using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LibProShip.Domain.Events;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Replay
{
    public class Installer: IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IInit,IDomainEventHandler<FileChangeEvent>>().ImplementedBy<RawFileProcessor>().LifestyleSingleton());
            container.Register(Component.For<ReplayRepository>().ImplementedBy<ReplayRepository>().LifestyleSingleton());
        }
    }
}