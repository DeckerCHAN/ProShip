using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibProShip.Domain.Events;
using LibProShip.Infrastructure.Configs;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.FileSystem
{
    public class FileMonitor : IFileMonitor, IInit
    {
        private readonly ISystemConfig Config;
        private readonly IEventBus EventBus;

        public FileMonitor(IEventBus eventBus, ISystemConfig config)
        {
            EventBus = eventBus;
            Config = config;
            RaisedFiles = new HashSet<string>();
        }

        private ISet<string> RaisedFiles { get; }


        public void TriggerScan()
        {
            var scannedFiles = GetAllReplayFile();
            scannedFiles = FilterOutExistReplays(scannedFiles);
            if (scannedFiles.Length == 0) return;

            RaiseNewReplayEvent(scannedFiles);
            SaveToProcessedReplay(scannedFiles);
        }

        public void Init()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    TriggerScan();
                }
            });
        }

        private void RaiseNewReplayEvent(ICollection<FileInfo> replayFiles)
        {
            var e = new FileChangeEvent(this, replayFiles);
            EventBus.Raise(e);
        }

        private FileInfo[] GetAllReplayFile()
        {
            var files = Config.ReplayPath.GetFiles("*.wowsreplay");
            return files;
        }

        private FileInfo[] FilterOutExistReplays(IEnumerable<FileInfo> files)
        {
            return files.Where(x => !RaisedFiles.Contains(x.Name)).ToArray();
        }

        private void SaveToProcessedReplay(IEnumerable<FileInfo> files)
        {
            var names = files.Select(x => x.Name);
            RaisedFiles.UnionWith(names);
        }
    }
}