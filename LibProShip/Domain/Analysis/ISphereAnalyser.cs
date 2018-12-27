using LibProShip.Domain.Analysis.Dto;
using LibProShip.Domain.StreamProcessor.Packet;

namespace LibProShip.Domain.Analysis
{
    public interface IAnalyser
    {
        string Name { get; }
        AnalysisCollection Analysis(BattleRecord battleRecord);
    }

 
}