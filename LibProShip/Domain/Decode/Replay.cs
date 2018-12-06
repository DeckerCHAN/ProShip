using System;
using System.Collections.Generic;

namespace LibProShip.Domain.Decode
{
    public sealed class Replay : IEntity<Replay>
    {
        public Battle Battle { get; }
        
        public double Version { get; }

        public DateTime DateTime { get; }

        public ICollection<Packet> Packets { get; }

        public Guid Identity { get; }
        
    }
}