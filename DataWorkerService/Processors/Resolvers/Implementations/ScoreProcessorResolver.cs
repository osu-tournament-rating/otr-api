using Database.Entities;
using DataWorkerService.Processors.Resolvers.Interfaces;
using DataWorkerService.Processors.Scores;

namespace DataWorkerService.Processors.Resolvers.Implementations;

public class ScoreProcessorResolver(
    IEnumerable<IProcessor<GameScore>> processors
) : ProcessorResolver<GameScore>(processors), IScoreProcessorResolver
{
    public override IProcessor<GameScore> GetAutomationChecksProcessor() =>
        Processors.FirstOrDefault(p => p is ScoreAutomationChecksProcessor)
        ?? throw new InvalidOperationException($"Processor was not registered: {nameof(ScoreAutomationChecksProcessor)}");

    public override IProcessor<GameScore> GetVerificationProcessor() =>
        Processors.FirstOrDefault(p => p is ScoreVerificationProcessor)
        ?? throw new InvalidOperationException($"Processor was not registered: {nameof(ScoreVerificationProcessor)}");
}
