using System.IO;

namespace LibProShip.Infrastructure.Configs
{
    public interface ISystemConfig
    {
        DirectoryInfo ReplayPath { get; }
    }


    public class SystemConfig :  ISystemConfig
    {



        public DirectoryInfo ReplayPath
        {
//            get { return new DirectoryInfo(this.GetConfig("ReplayPath")); }
            get { return new DirectoryInfo(@"D:\World_of_Warships\test_replays"); }
        }
    }
}