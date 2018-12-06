namespace LibProShip.Domain.Decode.Interface
{
    public interface IReplayAssembiler
    {
        Replay Assembly(RawReplay rawReplay);
    }
}