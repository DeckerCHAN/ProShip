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

        public IEnumerable<string> LoadedAnalysers()
        {
            return this.Analysers.Select(x => x.Name);
        }

        public void Analysis(string replayId, IEnumerable<string> analyserNames)
        {
            var replay = ReplayRepository.Find(r => r.Id == replayId).FirstOrDefault() ??
                         throw new ArgumentException();
            var binary = ReplayRepository.FindFile(replay);

            var battleRecord = StreamProcessors.Select(x =>
            {
                try
                {
                    return x.ProcessStream(binary.Clone() as byte[]);
                }
                catch (Exception)
                {
                    //TODO: Add logger
                    return null;
                }
            }).FirstOrDefault(x => x != null) ?? throw new Exception("Unable to process stream.");


            var analysis = Analysers
                .Where(x => analyserNames.Contains(x.Name));

            foreach (var analysi in analysis)
            {
                var collection = analysi.Analysis(battleRecord);
                replay.AnalysisResults[analysi.Name] = collection;
            }

            this.ReplayRepository.Update(replay);

            EventBus.Raise(new AnalysisUpdatedEvent(this, replayId));
        }
    }
}