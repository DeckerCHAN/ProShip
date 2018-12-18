using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LibProShip.Infrastructure.Configs;
using LibProShip.Infrastructure.Eventing;
using LibProShip.Infrastructure.Logging;

namespace LibProShip.Infrastructure
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IEventBus>().ImplementedBy<DefaultEventBus>().LifestyleSingleton());
            container.Register(Component.For<ISystemConfig>().ImplementedBy<SystemConfig>().LifestyleSingleton());
            container.Register(Component.For<ILogger>().ImplementedBy<DefaultLogger>().LifestyleTransient());
        }
    }
}