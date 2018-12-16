using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LibProShip.Domain.Decode.Event;
using LibProShip.Domain2;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Parse
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IInit, IDomainEventHandler<NewRawReplayEvent>>()
                .ImplementedBy<NewRawReplayHandler>().LifestyleSingleton());

            var processorTypes = Assembly.GetExecutingAssembly().GetTypes().ToList()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsInterface)
                .Where(x => typeof(IReplayProcessor).IsAssignableFrom(x));

            foreach (var processorType in processorTypes)
            {
                container.Register(Component.For<IReplayProcessor>().ImplementedBy(processorType).LifestyleSingleton());
            }
        }
    }
}