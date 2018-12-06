using LibProShip.Domain.Decode;

namespace LibProShip.Domain.FileSystem.Interface
{
    public interface IBattleMonitor
    {
        void RaiseNewReplayEvent();
        void RaiseNewBattleEvent();

        RawReplay[] ScanReplay();
    }
}