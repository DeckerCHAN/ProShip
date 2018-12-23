namespace LibProShip.Domain.StreamProcessor
{
    public interface IStreamProcessor
    {
        Packets.Packets ProcessStream(byte[] data);
    }

}