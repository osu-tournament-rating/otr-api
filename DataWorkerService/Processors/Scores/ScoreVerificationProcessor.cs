using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.Processors.Scores;

/// <summary>
/// Processor tasked with finalizing the <see cref="VerificationStatus"/> for
/// a <see cref="GameScore"/>
/// </summary>
public class ScoreVerificationProcessor(ILogger<ScoreVerificationProcessor> logger) : ProcessorBase<GameScore>(logger)
{
    protected override Task OnProcessingAsync(GameScore entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not ScoreProcessingStatus.NeedsVerification)
        {
            logger.LogTrace(
                "Score does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return Task.CompletedTask;
        }

        switch (entity.VerificationStatus)
        {
            case VerificationStatus.PreRejected or VerificationStatus.PreVerified:
                logger.LogTrace(
                    "Score is still awaiting verification [Id: {Id} | Processing Status: {Status}]",
                    entity.Id,
                    entity.ProcessingStatus
                );
                break;
            case VerificationStatus.Rejected or VerificationStatus.Verified:
                entity.ProcessingStatus = ScoreProcessingStatus.Done;
                break;
        }

        return Task.CompletedTask;
    }
}
