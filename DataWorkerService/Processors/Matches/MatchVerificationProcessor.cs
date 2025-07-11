using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Matches;

/// <summary>
/// Processor tasked with finalizing the <see cref="VerificationStatus"/> for
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

        switch (entity.Games.Count)
        {
            case > 0 when !entity.Games.All(g => g.ProcessingStatus > GameProcessingStatus.NeedsVerification):
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

                    break;
                }

            case 0:
                entity.VerificationStatus = VerificationStatus.Rejected;
                break;
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
                entity.RejectAllChildren();
                break;
            case VerificationStatus.Verified:
                entity.ProcessingStatus = MatchProcessingStatus.NeedsStatCalculation;
                break;
        }
    }
}
