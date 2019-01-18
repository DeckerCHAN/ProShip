namespace LibProShip.Domain.Replay
{
    public class ReplayDataContainer
    {
        public ReplayDataContainer(Replay replay, byte[] replayData)
        {
            ReplayData = replayData;
            Replay = replay;
        }

        public Replay Replay { get; }
        public byte[] ReplayData { get; }
    }
}