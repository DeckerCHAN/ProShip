using System;

namespace LibProShip.Infrastructure.Logging
{
    public interface ILogger
    {
        void Info(string info);
        void Debug(string debug);
        void Error(string error);
        void Error(Exception exception);
    }
}