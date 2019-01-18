using System.Collections.Generic;

namespace LibProShip.Domain.Analysis
{
    public interface IAnalysisManager
    {
        // We will raise event here
        void Analysis(string replayId, IEnumerable<string> analyserNames);
    }
}