using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.Installer;
using LibProShip.Application.VisualObject;
using LibProShip.Domain;
using LibProShip.Domain.Decode;
using LibProShip.Domain.Parse.Events;
using LibProShip.Infrastructure.Event;

namespace LibProShip.Application
{
    public class DomainEventPublisher : IDomainEventHandler<IDomainEvent>
    {
        private readonly IEventBus EventBus;

        public event EventHandler NewReplaySaved;

        public DomainEventPublisher(IEventBus eventBus)
        {
            this.EventBus = eventBus;
        }

        public void Handle(IDomainEvent e)
        {
            if (e.GetType() == typeof(NewReplaySavedEvent))
            {
                var handler = this.NewReplaySaved;
                handler?.Invoke(this, new EventArgs());
            }
            else
            {
            }
        }

        public void Init()
        {
            this.EventBus.Register(this);
        }
    }


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
            this.Container.Install(FromAssembly.This());
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