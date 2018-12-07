using System;
using System.Linq;
using LibProShip.Domain.Analysis.Interface;
using LibProShip.Domain.Decode;
using LibProShip.Infrastructure.Utils;

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
            if (VersionUtils.Lower(replay.Version, "0.6.0.0") || VersionUtils.Higher(replay.Version, "0.7.10.6"))
            {
                return null;
            }

            var packetSorted = replay.Packets.OrderBy(x => x.Time).ToList();

            var index = 0;

            while (index < packetSorted.Count)
            {
                var packet = packetSorted[index];
                if (packet.Data)
                {
                    //TODO: Do something
                }

                index++;
            }


            throw new NotImplementedException();
        }
    }
}