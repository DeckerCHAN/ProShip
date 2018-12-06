using System.Collections.Generic;
using System.IO;

namespace LibProShip.Domain.Decode.Interface
{
    public interface IStreamParser
    {
        ICollection<Packet> Parse(Stream stream);
    }
}