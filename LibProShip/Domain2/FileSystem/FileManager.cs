using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibProShip.Domain;
using LibProShip.Domain2.Events;
using LibProShip.Infrastructure.Configs;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain2.FileSystem
{
    public interface IFileMonitor
    {
        void TriggerScan();
    }

    public class FileMonitor : IFileMonitor, IInit
    {
        private readonly IEventBus EventBus;
        private readonly ISystemConfig Config;
        private ISet<string> RaisedFiles { get; }

        public FileMonitor(IEventBus eventBus, ISystemConfig config)
        {
            this.EventBus = eventBus;
            this.Config = config;
            this.RaisedFiles = new HashSet<string>();
        }

        private void RaiseNewReplayEvent(ICollection<FileInfo> replayFiles)
        {
            var e = new FileChangeEvent(this, replayFiles);
            this.EventBus.Raise(e);
        }

        private FileInfo[] GetAllReplayFile()
        {
            var files = this.Config.ReplayPath.GetFiles("*.wowsreplay");
            return files;
        }

        private FileInfo[] FilterOutExistReplays(IEnumerable<FileInfo> files)
        {
            return files.Where(x => !this.RaisedFiles.Contains(x.Name)).ToArray();
        }

        private void SaveToProcessedReplay(IEnumerable<FileInfo> files)
        {
            var names = files.Select(x => x.Name);
            this.RaisedFiles.UnionWith(names);
        }


        public void TriggerScan()
        {
            var scannedFiles = this.GetAllReplayFile();
            scannedFiles = this.FilterOutExistReplays(scannedFiles);
            if (scannedFiles.Length == 0)
            {
                return;
            }

            this.RaiseNewReplayEvent(scannedFiles);
            this.SaveToProcessedReplay(scannedFiles);
        }

        public void Init()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    this.TriggerScan();
                }
            });
        }
    }
}