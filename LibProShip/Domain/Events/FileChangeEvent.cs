using System.Collections.Generic;
using System.IO;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Events
{
    public class FileChangeEvent : IDomainEvent
    {
        public FileChangeEvent(object source, ICollection<FileInfo> replayFiles)
        {
            Source = source;
            ReplayFiles = replayFiles;
        }

        public ICollection<FileInfo> ReplayFiles { get; }

        public object Source { get; }
    }
}