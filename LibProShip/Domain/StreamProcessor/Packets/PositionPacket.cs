using System.Collections.Generic;

namespace LibProShip.Domain.StreamProcessor.Packets
{
    public sealed class Packets
    {
        public IEnumerable<PositionPacket> PositionPacket { get; private set; }
    }

    public sealed class PositionPacket
    {
        public int time { get; private set; }
    }

    public class ProjectilePacket
    {
        
    }
}