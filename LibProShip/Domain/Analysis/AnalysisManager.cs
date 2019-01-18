using System;
using System.Collections.Generic;
using System.Linq;
using LibProShip.Domain.Events;
using LibProShip.Domain.Replay;
using LibProShip.Domain.StreamProcessor;
using LibProShip.Infrastructure.Eventing;

namespace LibProShip.Domain.Analysis
{
    public class AnalysisManager : IAnalysisManager
    {
        private readonly IEnumerable<IAnalyser> Analysers;
        private readonly IEventBus EventBus;
        private readonly ReplayRepository ReplayRepository;
        private readonly IEnumerable<IStreamProcessor> StreamProcessors;

        public AnalysisManager(IEventBus eventBus,
            IEnumerable<IStreamProcessor> streamProcessors,
            IEnumerable<IAnalyser> analysers, ReplayRepository replayRepository)
        {
            StreamProcessors = streamProcessors;
            EventBus = eventBus;
            Analysers = analysers;
            ReplayRepository = replayRepository;
        }

        public void Analysis(string replayId, string analyserName)
        {
            var replay = ReplayRepository.Find(r => r.Id == replayId).FirstOrDefault() ??
                         throw new ArgumentException();
            var data = ReplayRepository.FindFile(replay);

            var st = StreamProcessors.Select(x =>
            {
                try
                {
                    return x.ProcessStream(data.Clone() as byte[]);
                }
                catch (Exception e)
                {
                    //TODO: Add logger
                    return null;
                }
            }).FirstOrDefault(x => x != null) ?? throw new Exception("Unable to process stream.");


            var analyser = Analysers.FirstOrDefault(x => x.Name.Equals(analyserName)) ??
                           throw new Exception($"Unable to find analyser {analyserName}");

            var result = analyser.Analysis(st);
            replay.AnalysisResults[analyser.Name] = result;

            EventBus.Raise(new AnalysisUpdatedEvent(this, replayId));
        }
    }
}