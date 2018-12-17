using System;
using System.IO;
using LibProShip.Domain.Decode;

namespace LibProShip.Domain2.Decoding
{
    public interface IDecoder
    {
        //TODO: Use a proper wrapper
        Tuple<Battle , byte []> DecodeReplay(FileInfo replayFile);
    }
}