using System.IO;

namespace LibProShip.Infrastructure.Configs
{
    public interface ISystemConfig
    {
        DirectoryInfo ReplayPath { get; }
    }


    public class SystemConfig : ISystemConfig
    {
        public DirectoryInfo ReplayPath => new DirectoryInfo(@"/home/derekchan/Downloads/replays");
    }
}