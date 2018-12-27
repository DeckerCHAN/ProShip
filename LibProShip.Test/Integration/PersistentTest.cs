using System.Collections.Generic;
using System.Linq;
using LibProShip.Domain.Analysis.Dto;
using LibProShip.Domain.Replay;
using LibProShip.Infrastructure.Utils;
using Xunit;
using Replay = LibProShip.Domain.Replay.Replay;

namespace LibProShip.Test.Integration
{
    public class PresistentTest
    {
        [Fact]
        public void Test1()
        {
            var rpData = new byte[] {12, 3, 45, 67, 1, 23, 42, 134, 55, 12};

            var rp = new Replay(HashUtils.Hash(rpData), "abc.wowosreplay", null, new Dictionary<string, AnalysisCollection>());

            var repo = new ReplayRepository();
            repo.Insert(rp, rpData);

            var restoredRp = repo.Find((r => r.Id == rp.Id)).FirstOrDefault();

            Assert.Equal(restoredRp.AnalysisResults.Count, rp.AnalysisResults.Count);

            var restoreData = repo.FindFile(restoredRp);

            Assert.Equal(restoreData[1], rpData[1]);
        }
    }
}