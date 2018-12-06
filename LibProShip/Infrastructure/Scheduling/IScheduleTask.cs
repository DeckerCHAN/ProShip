using System;

namespace LibProShip.Infrastructure.Scheduling
{
    public interface IScheduleTask
    {
        Action TaskExpression { get; }
    }
}