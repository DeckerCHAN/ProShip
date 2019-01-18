using System.Collections.Generic;
using LibProShip.Domain.Analysis.Result;
using LibProShip.Domain.Replay.Entities;

namespace LibProShip.Domain.Replay
{
    public sealed class Replay : Entity
    {
        public Replay()
        {
            // For ORM
        }

        public Replay(string id, string fileName, Battle battle,
            IDictionary<string, AnalysisCollection> analysisResults) : base(id)
        {
            AnalysisResults = analysisResults;
            FileName = fileName;
            Battle = battle;
        }

        public Battle Battle { get; }
        public string FileName { get; }
        public IDictionary<string, AnalysisCollection> AnalysisResults { get; }
    }
}