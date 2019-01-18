using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using LibProShip.Application.VisualObject;
using LibProShip.Domain;
using LibProShip.Domain.Replay;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Application
{
    public class Application
    {
        public Application()
        {
            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(Container.Kernel));

            Container.Install(FromAssembly.InThisApplication());
            EventBus = Container.Resolve<IEventBus>();
            DomainEventPublisher = Container.Resolve<DomainEventPublisher>();
            Initialise();
        }

        //TODO: Remove container init here
        //It is anti-pattern and harder to test
        private IWindsorContainer Container { get; }
        private IEventBus EventBus { get; }
        public DomainEventPublisher DomainEventPublisher { get; }

        private void Initialise()
        {
            foreach (var init in Container.ResolveAll<IInit>()) init.Init();
        }

        public async Task<IEnumerable<ReplayAbstract>> GetReplays(int itemsPrePage, int pageNumber)
        {
            return await Task.Run(() =>
                {
                    var page = Container.Resolve<ReplayRepository>().Paging(itemsPrePage, pageNumber);
                    var absPage = page.Select(x =>
                        new ReplayAbstract(x.Battle.DateTime.ToString("f"),
                            x.Battle.Vehicles.First(y => y.ShipId == x.Battle.ControlVehicle.ShipId).ToString(), x.Id));

                    return absPage;
                }
            );
        }
    }
}