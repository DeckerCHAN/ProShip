using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using LibProShip.Application.VisualObject;
using LibProShip.Domain;
using LibProShip.Domain.Decode;
using LibProShip.Domain.Decode.Handler;
using LibProShip.Domain.Parse;
using LibProShip.Infrastructure.Eventing;
using LibProShip.Infrastructure.Repo;

namespace LibProShip.Application
{
    public class Application
    {
        //TODO: Remove container init here
        //It is anti-pattern and harder to test
        private IWindsorContainer Container { get; }
        private IEventBus EventBus { get; }
        public DomainEventPublisher DomainEventPublisher { get; }

        public Application()
        {
            this.Container = new WindsorContainer();
            this.Container.Kernel.Resolver.AddSubResolver(new CollectionResolver( this.Container.Kernel));

            this.Container.Install(FromAssembly.InThisApplication());
            this.EventBus = this.Container.Resolve<IEventBus>();
            this.DomainEventPublisher = this.Container.Resolve<DomainEventPublisher>();
            this.Initialise();
        }

        private void Initialise()
        {
            foreach (var init in this.Container.ResolveAll<IInit>())
            {
                init.Init();
            }
        }

        public async Task<IReadOnlyCollection<ReplayAbstract>> GetReplays()
        {
            return await Task.Run((() =>
            {

                var replayAbstractReplayAbstracts = this.Container.Resolve<IRepository<Replay>>().GetAll().Select(x =>
                    new ReplayAbstract(x.DateTime.ToString("f"),
                        x.Battle.PlayerShips[x.Battle.ControlPlayer].ToString()));
                
                return new ReadOnlyCollection<ReplayAbstract>(replayAbstractReplayAbstracts.ToList());
                
            }));
        }
    }
}