using LibProShip.Domain.Parse;

namespace LibProShip.Domain.Decode.Factory
{
    public interface IDecoderFactory
    {
        IReplayProcessor GetDecoder();
    }
}