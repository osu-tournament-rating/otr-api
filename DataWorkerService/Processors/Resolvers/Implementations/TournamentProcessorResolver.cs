using Common.Enums.Enums.Verification;
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
        ?? throw new InvalidOperationException(
            $"Processor was not registered: {nameof(TournamentAutomationChecksProcessor)}");

    public IProcessor<Tournament> GetStatsProcessor() =>
        Processors.FirstOrDefault(p => p is TournamentStatsProcessor)
        ?? throw new InvalidOperationException($"Processor was not registered: {nameof(TournamentStatsProcessor)}");

    public IProcessor<Tournament> GetNextProcessor(TournamentProcessingStatus processingStatus) =>
        processingStatus switch
        {
            TournamentProcessingStatus.NeedsMatchData => GetDataProcessor(),
            TournamentProcessingStatus.NeedsAutomationChecks => GetAutomationChecksProcessor(),
            TournamentProcessingStatus.NeedsStatCalculation => GetStatsProcessor(),
            TournamentProcessingStatus.NeedsVerification => GetVerificationProcessor(),
            _ => throw new ArgumentException(
                $"No next processor is known for the TournamentProcessingStatus {processingStatus}")
        };

    public override IProcessor<Tournament> GetVerificationProcessor() =>
        Processors.FirstOrDefault(p => p is TournamentVerificationProcessor)
        ?? throw new InvalidOperationException(
            $"Processor was not registered: {nameof(TournamentVerificationProcessor)}");
}
