using System;
using System.IO;
using LibProShip.Domain.Replay.Entities;

namespace LibProShip.Domain.Decoding
{
    public interface IDecoder
    {
        //TODO: Use a proper wrapper
        Tuple<Battle, byte[]> DecodeReplay(FileInfo replayFile);
    }
}