using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks;

namespace DataWorkerService.Processors.Scores;

/// <summary>
/// Processor tasked with running <see cref="AutomationChecks.IAutomationCheck"/>s against a <see cref="GameScore"/>
/// </summary>
public class ScoreAutomationChecksProcessor(
    ILogger<ScoreAutomationChecksProcessor> logger,
    IEnumerable<IAutomationCheck<GameScore>> scoreAutomationChecks
) : ProcessorBase<GameScore>(logger)
{
    protected override Task OnProcessingAsync(GameScore entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not ScoreProcessingStatus.NeedsAutomationChecks)
        {
            logger.LogTrace(
                "Score does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return Task.CompletedTask;
        }

        foreach (IAutomationCheck<GameScore> automationCheck in scoreAutomationChecks.OrderBy(ac => ac.Order))
        {
            automationCheck.Check(entity);
        }

        entity.VerificationStatus = entity.RejectionReason is ScoreRejectionReason.None
            ? VerificationStatus.PreVerified
            : VerificationStatus.PreRejected;

        entity.ProcessingStatus = ScoreProcessingStatus.NeedsVerification;

        return Task.CompletedTask;
    }
}
