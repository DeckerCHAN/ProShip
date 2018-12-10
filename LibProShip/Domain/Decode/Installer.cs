using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LibProShip.Domain.Decode.Handler;
using LibProShip.Domain.FileSystem;
using LibProShip.Domain.Parse;
using LibProShip.Infrastructure.Eventing;
using LibProShip.Infrastructure.Repo;

namespace LibProShip.Domain.Decode
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IInit, IDomainEventHandler<FileChangeEvent>,RawFileProcessor>()
                .ImplementedBy<RawFileProcessor>().LifestyleSingleton());

            container.Register(Component.For<IRepository<Replay>>().ImplementedBy<RepositoryBase<Replay>>()
                .LifestyleSingleton());

            var decoderTypes = Assembly.GetExecutingAssembly().GetTypes().ToList()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsInterface)
                .Where(x => typeof(IDecoder).IsAssignableFrom(x));

            foreach (var processorType in decoderTypes)
            {
                container.Register(Component.For<IDecoder>().ImplementedBy(processorType).LifestyleTransient());
            }
        }
    }
}