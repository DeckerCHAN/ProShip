using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibProShip.Infrastructure.Scheduling
{
    public class TaskScheduler : ITaskScheduler
    {
        public TaskScheduler()
        {
        }

        public async void AddDelayTask(Action action, TimeSpan delay, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async void AddRecurringTask(Action action, TimeSpan interval, CancellationToken ct)
        {
            await Task.Run(() =>
            {
                while (ct.IsCancellationRequested)
                {
                    action();
                    Thread.Sleep(interval);
                }
            }, ct);
        }
    }
}