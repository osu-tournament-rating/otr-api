using Database.Entities;
using Database.Enums.Verification;
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
            logger.LogDebug(
                "Match does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        if (!entity.Games.All(g => g.ProcessingStatus > GameProcessingStatus.NeedsVerification))
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
                break;
            case VerificationStatus.Verified:
                entity.ProcessingStatus = MatchProcessingStatus.NeedsStatCalculation;
                break;
        }
    }
}
