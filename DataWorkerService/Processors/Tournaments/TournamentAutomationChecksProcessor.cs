using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Tournaments;

/// <summary>
/// Processor tasked with running <see cref="AutomationChecks.IAutomationCheck"/>s against a <see cref="Tournament"/>
/// </summary>
public class TournamentAutomationChecksProcessor(
    ILogger<TournamentAutomationChecksProcessor> logger,
    IEnumerable<IAutomationCheck<Tournament>> tournamentAutomationChecks,
    IMatchProcessorResolver matchProcessorResolver
) : ProcessorBase<Tournament>(logger)
{
    protected override async Task OnProcessingAsync(Tournament entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not TournamentProcessingStatus.NeedsAutomationChecks)
        {
            logger.LogDebug(
                "Tournament does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        IProcessor<Match> matchAutomationChecksProcessor = matchProcessorResolver.GetAutomationChecksProcessor();
        IEnumerable<Task> tasks = entity.Matches
            .Select(m => matchAutomationChecksProcessor.ProcessAsync(m, cancellationToken));
        await Task.WhenAll(tasks);

        foreach (IAutomationCheck<Tournament> automationCheck in tournamentAutomationChecks.OrderBy(ac => ac.Order))
        {
            automationCheck.Check(entity);
        }

        entity.VerificationStatus = entity.RejectionReason is TournamentRejectionReason.None
            ? VerificationStatus.PreVerified
            : VerificationStatus.PreRejected;

        entity.ProcessingStatus = TournamentProcessingStatus.NeedsVerification;
    }
}
