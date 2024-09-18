using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Games;

/// <summary>
/// Processor tasked with running <see cref="AutomationChecks.IAutomationCheck"/>s against a <see cref="Game"/>
/// </summary>
public class GameAutomationChecksProcessor(
    ILogger<GameAutomationChecksProcessor> logger,
    IEnumerable<IAutomationCheck<Game>> gameAutomationChecks,
    IScoreProcessorResolver scoreProcessorResolver
) : ProcessorBase<Game>(logger)
{
    protected override async Task OnProcessingAsync(Game entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not GameProcessingStatus.NeedsAutomationChecks)
        {
            logger.LogDebug(
                "Game does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        IProcessor<GameScore> scoreAutomationChecksProcessor = scoreProcessorResolver.GetAutomationChecksProcessor();

        foreach (GameScore score in entity.Scores)
        {
            await scoreAutomationChecksProcessor.ProcessAsync(score, cancellationToken);
        }

        foreach (IAutomationCheck<Game> automationCheck in gameAutomationChecks.OrderBy(ac => ac.Order))
        {
            automationCheck.Check(entity);
        }

        entity.VerificationStatus = entity.RejectionReason is GameRejectionReason.None
            ? VerificationStatus.PreVerified
            : VerificationStatus.PreRejected;

        entity.ProcessingStatus = GameProcessingStatus.NeedsVerification;
    }
}
