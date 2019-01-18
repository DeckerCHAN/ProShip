using System;
using System.Threading;
using LibProShip.Infrastructure.Eventing;
using Xunit;

namespace LibProShip.Test.Unit.EventTest
{
    public class EventBusTest
    {
        [Fact]
        public void ItShouldHandleEventOnce()
        {
            IEventBus eventBus = new DefaultEventBus();
            var handlerMock = new TestDomainEventHandler();
            eventBus.Register(handlerMock);
            eventBus.Raise(new TestEvent());

            Thread.Sleep(5000);

            Assert.Equal(handlerMock.HandleTimes, 1);
        }
    }

    internal class TestDomainEventHandler : IDomainEventHandler<TestEvent>
    {
        public TestDomainEventHandler()
        {
            HandleTimes = 0;
        }

        public int HandleTimes { get; set; }

        public void Handle(TestEvent e)
        {
            HandleTimes++;
        }

        public void Init()
        {
            //This should not be called
            throw new NotImplementedException();
        }
    }

    internal class TestEvent : IDomainEvent
    {
        public object Source { get; }
    }
}