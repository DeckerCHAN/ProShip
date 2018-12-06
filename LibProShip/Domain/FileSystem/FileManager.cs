using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Castle.DynamicProxy;
using LibProShip.Infrastructure.Configs;
using LibProShip.Infrastructure.Event;
using LibProShip.Infrastructure.Scheduling;

namespace LibProShip.Domain.FileSystem
{
    public interface IFileManager
    {
        void TriggerScan();
    }

    public class FileManager : IFileManager, IInit
    {
        private readonly IEventBus EventBus;
        private readonly ISystemConfig Config;
        private readonly ITaskScheduler Scheduler;
        private ISet<string> RaisedFiles { get; }

        public FileManager(IEventBus eventBus, ISystemConfig config, ITaskScheduler scheduler)
        {
            this.EventBus = eventBus;
            this.Config = config;
            this.Scheduler = scheduler;
            this.RaisedFiles = new HashSet<string>();
        }

        private void RaiseNewReplayEvent(ICollection<FileInfo> replayFiles)
        {
            var e = new FileChangeDomainEvent(this, replayFiles);
            this.EventBus.Raise(e);
        }

        private FileInfo[] GetAllReplayFile()
        {
            var files = this.Config.ReplayPath.GetFiles("*.replay");
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
            this.RaiseNewReplayEvent(scannedFiles);
            this.SaveToProcessedReplay(scannedFiles);
        }

        public void Init()
        {
            this.Scheduler.AddRecurringTask(this.TriggerScan, TimeSpan.FromSeconds(10),
                new CancellationToken());
        }
    }
}