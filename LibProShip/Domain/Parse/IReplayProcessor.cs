using LibProShip.Domain.Decode;

namespace LibProShip.Domain.Parse
{
    public interface IReplayProcessor
    {
        Replay Process(RawReplay rawReplay);
    }
}