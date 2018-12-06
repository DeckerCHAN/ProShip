using System.Collections.Generic;
using System.IO;
using LibProShip.Infrastructure.Event;

namespace LibProShip.Domain.FileSystem
{
    public class FileChangeDomainEvent : IDomainEvent
    {
        public FileChangeDomainEvent(object source, ICollection<FileInfo> replayFiles)
        {
            this.Source = source;
            this.ReplayFiles = replayFiles;
        }

        public object Source { get; }
        public ICollection<FileInfo> ReplayFiles { get; }
    }
}