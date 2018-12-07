using System.Threading;
using LibProShip.Infrastructure.Event;
using Xunit;

namespace LibProShip.Test.Integration.EventTest
{
    public class EventBusTest
    {
        public EventBusTest()
        {
        }


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
        public int HandleTimes { get; set; }

        public TestDomainEventHandler()
        {
            this.HandleTimes = 0;
        }

        public void Handle(TestEvent e)
        {
            this.HandleTimes++;
        }

        public void Init()
        {
            //This should not be called
            throw new System.NotImplementedException();
        }
    }

    internal class TestEvent : IDomainEvent
    {
        public object Source { get; }
    }
}