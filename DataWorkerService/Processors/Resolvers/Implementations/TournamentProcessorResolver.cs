using Database.Entities;
using DataWorkerService.Processors.Resolvers.Interfaces;
using DataWorkerService.Processors.Tournaments;

namespace DataWorkerService.Processors.Resolvers.Implementations;

public class TournamentProcessorResolver(
    IEnumerable<IProcessor<Tournament>> processors
) : ProcessorResolver<Tournament>(processors), ITournamentProcessorResolver
{
    public IProcessor<Tournament> GetDataProcessor() =>
        Processors.FirstOrDefault(p => p is TournamentDataProcessor)
            ?? throw new InvalidOperationException($"Processor was not registered: {nameof(TournamentDataProcessor)}");

    public override IProcessor<Tournament> GetAutomationChecksProcessor() =>
        Processors.FirstOrDefault(p => p is TournamentAutomationChecksProcessor)
        ?? throw new InvalidOperationException($"Processor was not registered: {nameof(TournamentAutomationChecksProcessor)}");

    public IProcessor<Tournament> GetStatsProcessor() =>
        Processors.FirstOrDefault(p => p is TournamentStatsProcessor)
        ?? throw new InvalidOperationException($"Processor was not registered: {nameof(TournamentStatsProcessor)}");
}
