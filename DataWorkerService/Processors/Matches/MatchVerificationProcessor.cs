using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.Processors.Games;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Matches;

/// <summary>
/// Processor tasked with finalizing the <see cref="Database.Enums.Verification.VerificationStatus"/> for
/// a <see cref="Match"/>
/// </summary>
public class MatchVerificationProcessor(
    ILogger<MatchVerificationProcessor> logger,
    IGameProcessorResolver gameProcessorResolver
) : ProcessorBase<Match>(logger)
{
    protected override async Task OnProcessingAsync(Match entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not MatchProcessingStatus.NeedsVerification)
        {
            logger.LogTrace(
                "Match does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        if (entity.Games.Count > 0 && !entity.Games.All(g => g.ProcessingStatus > GameProcessingStatus.NeedsVerification))
        {
            IProcessor<Game> gameVerificationProcessor = gameProcessorResolver.GetVerificationProcessor();
            foreach (Game game in entity.Games)
            {
                await gameVerificationProcessor.ProcessAsync(game, cancellationToken);
            }

            if (!entity.Games.All(g => g.ProcessingStatus > GameProcessingStatus.NeedsVerification))
            {
                logger.LogDebug(
                    "Match's games are still awaiting verification [Id: {Id} | Processing Status: {Status}]",
                    entity.Id,
                    entity.ProcessingStatus
                );

                return;
            }
        }
        else if (entity.Games.Count == 0)
        {
            entity.VerificationStatus = VerificationStatus.Rejected;
        }

        switch (entity.VerificationStatus)
        {
            case VerificationStatus.PreRejected or VerificationStatus.PreVerified:
                logger.LogDebug(
                    "Match is still awaiting verification [Id: {Id} | Processing Status: {Status}]",
                    entity.Id,
                    entity.ProcessingStatus
                );
                break;
            case VerificationStatus.Rejected:
                entity.ProcessingStatus = MatchProcessingStatus.Done;
                RejectAllChildren(entity);
                break;
            case VerificationStatus.Verified:
                entity.ProcessingStatus = MatchProcessingStatus.NeedsStatCalculation;
                break;
        }
    }

    /// <summary>
    /// Rejects all child entities in a <see cref="Match"/>
    /// </summary>
    public static void RejectAllChildren(Match match)
    {
        foreach (Game game in match.Games)
        {
            game.VerificationStatus = VerificationStatus.Rejected;
            game.RejectionReason |= GameRejectionReason.RejectedMatch;
            game.ProcessingStatus = GameProcessingStatus.Done;

            GameVerificationProcessor.RejectAllChildren(game);
        }
    }
}
