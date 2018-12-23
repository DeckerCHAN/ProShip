using System.Collections.Generic;
using System.Linq;
using LibProShip.Domain.Replay;
using LibProShip.Domain.StreamProcessor;

namespace LibProShip.Domain.Analysis
{
    public class AnalysisManager
    {
        private readonly IEnumerable<IStreamProcessor> StreamProcessors;
        private readonly IEnumerable<IAnalyser> Analysers;

        public AnalysisManager(IEnumerable<IStreamProcessor> streamProcessors, IEnumerable<IAnalyser> analysers)
        {
            this.StreamProcessors = streamProcessors;
            this.Analysers = analysers;
        }

        public T Analysis<T>(string analyserName, ReplayDataContainer container)
        {
            var foundAnalyser = this.Analysers.FirstOrDefault(x => x.GetType().Name.Equals(analyserName));
            
            foundAnalyser.
        }
    }
}