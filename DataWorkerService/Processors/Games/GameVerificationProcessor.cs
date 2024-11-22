using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Games;

/// <summary>
/// Processor tasked with finalizing the <see cref="Database.Enums.Verification.VerificationStatus"/> for
/// a <see cref="Game"/>
/// </summary>
public class GameVerificationProcessor(
    ILogger<GameVerificationProcessor> logger,
    IScoreProcessorResolver scoreProcessorResolver
) : ProcessorBase<Game>(logger)
{
    protected override async Task OnProcessingAsync(Game entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not GameProcessingStatus.NeedsVerification)
        {
            logger.LogTrace(
                "Game does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        if (!entity.Scores.All(s => s.ProcessingStatus > ScoreProcessingStatus.NeedsVerification))
        {
            IProcessor<GameScore> scoreVerificationProcessor = scoreProcessorResolver.GetVerificationProcessor();
            foreach (GameScore score in entity.Scores)
            {
                await scoreVerificationProcessor.ProcessAsync(score, cancellationToken);
            }

            if (!entity.Scores.All(s => s.ProcessingStatus > ScoreProcessingStatus.NeedsVerification))
            {
                logger.LogTrace(
                    "Game's scores are still awaiting verification [Id: {Id} | Processing Status: {Status}]",
                    entity.Id,
                    entity.ProcessingStatus
                );

                return;
            }
        }

        switch (entity.VerificationStatus)
        {
            case VerificationStatus.PreRejected or VerificationStatus.PreVerified:
                logger.LogTrace(
                    "Game is still awaiting verification [Id: {Id} | Processing Status: {Status}]",
                    entity.Id,
                    entity.ProcessingStatus
                );
                break;
            case VerificationStatus.Rejected:
                entity.ProcessingStatus = GameProcessingStatus.Done;
                RejectAllChildren(entity);
                break;
            case VerificationStatus.Verified:
                entity.ProcessingStatus = GameProcessingStatus.NeedsStatCalculation;
                break;
        }
    }

    /// <summary>
    /// Rejects all child entities in a <see cref="Game"/>
    /// </summary>
    public static void RejectAllChildren(Game game)
    {
        foreach (GameScore score in game.Scores)
        {
            score.VerificationStatus = VerificationStatus.Rejected;
            score.RejectionReason |= ScoreRejectionReason.RejectedGame;
            score.ProcessingStatus = ScoreProcessingStatus.Done;
        }
    }
}
