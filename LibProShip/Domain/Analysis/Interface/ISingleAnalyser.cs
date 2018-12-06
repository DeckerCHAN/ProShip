using System;
using System.Collections.Generic;
using LibProShip.Domain.Decode;

namespace LibProShip.Domain.Analysis.Interface
{
    public interface IAnalysisResult
    {
        DateTime ProducedDate { get; }
    }

    public interface ISingleAnalyser<out T> where T : IAnalysisResult
    {
        T GetAnalysisResult(Replay replay);
    }

    public interface IMultiAnalyser<out T> where T : IAnalysisResult
    {
        T GetAnalysisResult(IEnumerable<Replay> replays);
    }

    public class SingleValueResult : IAnalysisResult, IEntity<SingleValueResult>
    {
        public Guid Identity { get; }
        public DateTime ProducedDate { get; }
    }
}