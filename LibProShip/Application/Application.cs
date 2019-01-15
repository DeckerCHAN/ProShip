using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
//            return await Task.Run((() =>
//            {
//                this.Container.Resolve<ReplayRepository>().
//
//                var replayAbstractReplayAbstracts = this.Container.Resolve<ReplayRepository>().GetAll().Select(x =>
//                    new ReplayAbstract(x.DateTime.ToString("f"),
//                        x.Battle.PlayerVehicles[x.Battle.ControlPlayer].ToString()));
//                
//                return new ReadOnlyCollection<ReplayAbstract>(replayAbstractReplayAbstracts.ToList());
//                
//            }));
        }
    }
}