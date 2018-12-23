using LibProShip.Domain.Analysis.Dto;
using LibProShip.Domain.StreamProcessor.Packets;

namespace LibProShip.Domain.Analysis
{
    public interface IAnalyser
    {
        
    }

    public interface ISphereAnalyser : IAnalyser
    {
        SphereChart SphereAnalysis(Packets packets);
    }
}