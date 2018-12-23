using System.Collections.Generic;
using LibProShip.Domain.Analysis.Dto;
using LibProShip.Domain.Replay.Entities;

namespace LibProShip.Domain.Replay
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
            this.FileName = fileName;
            this.Battle = battle;
        }

        public Battle Battle { get; private set; }
        public string FileName { get; private set; }
        public IDictionary<string, AnalysisResult> AnalysisResult { get; private set; }
    }
}