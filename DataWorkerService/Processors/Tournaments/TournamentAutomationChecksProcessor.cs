using Database;
using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Tournaments;

/// <summary>
/// Processor tasked with running <see cref="AutomationChecks.IAutomationCheck"/>s against a <see cref="Tournament"/>
/// </summary>
public class TournamentAutomationChecksProcessor(
    ILogger<TournamentAutomationChecksProcessor> logger,
    IEnumerable<IAutomationCheck<Tournament>> tournamentAutomationChecks,
    IMatchProcessorResolver matchProcessorResolver,
    OtrContext context
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

        foreach (Match match in entity.Matches)
        {
            await matchAutomationChecksProcessor.ProcessAsync(match, cancellationToken);
        }

        foreach (IAutomationCheck<Tournament> automationCheck in tournamentAutomationChecks.OrderBy(ac => ac.Order))
        {
            automationCheck.Check(entity);
        }

        entity.VerificationStatus = entity.RejectionReason is TournamentRejectionReason.None
            ? VerificationStatus.PreVerified
            : VerificationStatus.PreRejected;

        entity.ProcessingStatus = TournamentProcessingStatus.NeedsVerification;

        await context.SaveChangesAsync(cancellationToken);
    }
}
