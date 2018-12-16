using System.IO;

namespace LibProShip.Domain2.Decoding
{
    public interface IDecoder
    {
        Replay.Replay DecodeReplay(FileInfo replayFile);
    }
}