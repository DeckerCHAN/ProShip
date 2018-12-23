namespace LibProShip.Domain.Replay
{
    public class ReplayDataContainer
    {
        public ReplayDataContainer(Replay replay, byte[] replayData)
        {
            this.ReplayData = replayData;
            this.Replay = replay;
        }

        public Replay Replay { get; private set; }
        public byte [] ReplayData { get; private set; }
    }
}