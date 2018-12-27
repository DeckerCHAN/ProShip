using LibProShip.Domain.StreamProcessor.Packet;

namespace LibProShip.Domain.StreamProcessor
{
    public interface IStreamProcessor
    {
        BattleRecord ProcessStream(byte[] data);
    }

}