using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks;
using DataWorkerService.AutomationChecks.Matches;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Matches;

/// <summary>
/// Processor tasked with running <see cref="AutomationChecks.IAutomationCheck"/>s against a <see cref="Match"/>
/// </summary>
public class MatchAutomationChecksProcessor(
    ILogger<MatchAutomationChecksProcessor> logger,
    IEnumerable<IAutomationCheck<Match>> matchAutomationChecks,
    IGameProcessorResolver gameProcessorResolver
) : ProcessorBase<Match>(logger)
{
    protected override async Task OnProcessingAsync(Match entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not MatchProcessingStatus.NeedsAutomationChecks)
        {
            logger.LogTrace(
                "Match does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        // Run MatchHeadToHeadCheck first, before processing games
        // This allows it to convert HeadToHead games to TeamVS before GameTeamTypeCheck rejects them
        var headToHeadCheck = matchAutomationChecks
            .OfType<MatchHeadToHeadCheck>()
            .FirstOrDefault();

        headToHeadCheck?.Check(entity);

        // Process games after potential HeadToHead conversion
        IProcessor<Game> gameAutomationChecksProcessor = gameProcessorResolver.GetAutomationChecksProcessor();

        foreach (Game game in entity.Games)
        {
            await gameAutomationChecksProcessor.ProcessAsync(game, cancellationToken);
        }

        // Run remaining match automation checks (excluding MatchHeadToHeadCheck which already ran)
        var remainingChecks = matchAutomationChecks
            .Where(ac => ac is not MatchHeadToHeadCheck)
            .OrderBy(ac => ac.Order);

        foreach (IAutomationCheck<Match> automationCheck in remainingChecks)
        {
            automationCheck.Check(entity);
        }

        entity.VerificationStatus = entity.RejectionReason is MatchRejectionReason.None
            ? VerificationStatus.PreVerified
            : VerificationStatus.PreRejected;

        entity.ProcessingStatus = MatchProcessingStatus.NeedsVerification;
    }
}
