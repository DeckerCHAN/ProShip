using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibProShip.Domain.Decode
{
    public sealed class Replay : Entity<Replay>
    {
        public Battle Battle { get; }
        
        public string Version { get; }

        public DateTime DateTime { get; }

        public IReadOnlyCollection<Packet> Packets { get; }

        public Replay(Guid id, Battle battle, DateTime dateTime, IEnumerable<Packet> packets) : base(id)
        {
            this.Battle = battle;
            this.DateTime = dateTime;
            this.Packets = new ReadOnlyCollection<Packet>(packets.ToList());
        }
    }
}