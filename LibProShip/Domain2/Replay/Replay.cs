using System.Collections.Generic;
using LibProShip.Domain.Decode;
using LibProShip.Domain2.Analysis;

namespace LibProShip.Domain2.Replay
{
    public sealed class Replay : Entity
    {
        public Replay()
        {
            //For ORM
        }

        public Replay(string id, string fileName, Battle battle, IDictionary<string, AnalysisResult> analysisResult) : base(id)
        {
            this.AnalysisResult = analysisResult;
            FileName = fileName;
            this.Battle = battle;
        }

        public Battle Battle { get; private set; }
        public string FileName { get; private set; }
        public IDictionary<string, AnalysisResult> AnalysisResult { get; private set; }
    }
}