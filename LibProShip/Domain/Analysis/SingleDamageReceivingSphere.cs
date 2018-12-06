using System;
using System.Linq;
using LibProShip.Domain.Analysis.Interface;
using LibProShip.Domain.Decode;

namespace LibProShip.Domain.Analysis
{
    public class SingleDamageReceivingSphereResult : IAnalysisResult
    {
        public DateTime ProducedDate { get; }
    }


    public class SingleDamageReceivingSphere : ISingleAnalyser<SingleDamageReceivingSphereResult>
    {
        public SingleDamageReceivingSphereResult GetAnalysisResult(Replay replay)
        {
            var packetSorted = replay.Packets.OrderBy(x => x.Time).ToList();

            var index = 0;

            while (index < packetSorted.Count)
            {
                var packet = packetSorted[index];
                if(packet.Data)
                {
                    
                    //TODO: Do something
                }
                
                index ++;
            }


            throw new NotImplementedException();
        }
    }
}