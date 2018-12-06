using System;
using System.Threading;
using LibProShip.Infrastructure.Scheduling;
using Xunit;
using Xunit.Abstractions;

namespace LibProShip.Test.Integration
{
    public class TaskScheduallerTest
    {
        private ITestOutputHelper output;
        
        public TaskScheduallerTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            var schedualler = new TaskScheduler();
            var cts = new CancellationTokenSource();
            schedualler.AddRecurringTask(() => { this.output.WriteLine(DateTime.Now.ToString("T"));}, TimeSpan.FromSeconds(1),cts.Token);
            Thread.Sleep(5000);
            cts.Cancel();            
        }
    }
}