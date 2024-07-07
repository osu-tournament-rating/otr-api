using Database.Entities;
using DataWorkerService.Processors.Games;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Resolvers.Implementations;

public class GameProcessorResolver(
    IEnumerable<IProcessor<Game>> processors
) : ProcessorResolver<Game>(processors), IGameProcessorResolver
{
    public override IProcessor<Game> GetAutomationChecksProcessor() =>
        Processors.FirstOrDefault(p => p is GameAutomationChecksProcessor)
        ?? throw new InvalidOperationException($"Processor was not registered: {nameof(GameAutomationChecksProcessor)}");
}
