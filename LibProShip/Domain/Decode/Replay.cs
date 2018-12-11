using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibProShip.Domain.Decode
{
    public sealed class Replay : Entity<Replay>
    {
        public Battle Battle { get; private set; }

        public string Version => this.Battle.Version;

        public DateTime DateTime { get; private set; }

        public IReadOnlyCollection<Packet> Packets { get; private set; }

        public Replay()
        {
            this.Battle = new Battle();
            this.DateTime = DateTime.MinValue;
            this.Packets = new ReadOnlyCollection<Packet>(new List<Packet>());
        }

        public Replay(Guid id, Battle battle, DateTime dateTime, IEnumerable<Packet> packets) : base(id)
        {
            this.Battle = battle;
            this.DateTime = dateTime;
            this.Packets = new ReadOnlyCollection<Packet>(packets.ToList());
        }
    }
}