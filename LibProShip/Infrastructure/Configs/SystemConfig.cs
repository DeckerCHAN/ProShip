using System.IO;

namespace LibProShip.Infrastructure.Configs
{
    public interface ISystemConfig
    {
        DirectoryInfo ReplayPath { get; }
    }


    public class SystemConfig : JsonDynamicConfig, ISystemConfig
    {
        public SystemConfig(string defaultJson) : base(defaultJson)
        {
        }


        public DirectoryInfo ReplayPath
        {
            get { return new DirectoryInfo(this.GetConfig("ReplayPath")); }
        }
    }
}