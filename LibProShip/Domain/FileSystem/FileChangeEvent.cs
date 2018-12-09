using System.Collections.Generic;
using System.IO;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.FileSystem
{
    public class FileChangeEvent : IDomainEvent
    {
        public FileChangeEvent(object source, ICollection<FileInfo> replayFiles)
        {
            this.Source = source;
            this.ReplayFiles = replayFiles;
        }

        public object Source { get; }
        public ICollection<FileInfo> ReplayFiles { get; }
    }
}