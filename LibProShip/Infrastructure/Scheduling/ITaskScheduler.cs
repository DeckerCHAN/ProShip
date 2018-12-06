using System;
using System.Threading;

namespace LibProShip.Infrastructure.Scheduling
{
    public interface ITaskScheduler
    {
        void AddDelayTask(Action action, TimeSpan delay, CancellationToken ct);

        void AddRecurringTask(Action action, TimeSpan interval, CancellationToken ct);
    }
}