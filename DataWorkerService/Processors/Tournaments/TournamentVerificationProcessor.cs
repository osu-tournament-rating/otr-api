using Database;
using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Tournaments;

/// <summary>
/// Processor tasked with finalizing the <see cref="Database.Enums.Verification.VerificationStatus"/> for
/// a <see cref="Tournament"/>
/// </summary>
public class TournamentVerificationProcessor(
    ILogger<TournamentVerificationProcessor> logger,
    IMatchProcessorResolver matchProcessorResolver,
    OtrContext context
) : ProcessorBase<Tournament>(logger)
{
    protected override async Task OnProcessingAsync(Tournament entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not TournamentProcessingStatus.NeedsVerification)
        {
            logger.LogDebug(
                "Tournament does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        if (!entity.Matches.All(m => m.ProcessingStatus > MatchProcessingStatus.NeedsVerification))
        {
            IProcessor<Match> matchVerificationProcessor = matchProcessorResolver.GetVerificationProcessor();
            foreach (Match match in entity.Matches)
            {
                await matchVerificationProcessor.ProcessAsync(match, cancellationToken);
            }

            if (!entity.Matches.All(m => m.ProcessingStatus > MatchProcessingStatus.NeedsVerification))
            {
                logger.LogDebug(
                    "Tournament's matches are still awaiting verification [Id: {Id} | Processing Status: {Status}]",
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
                    "Tournament is still awaiting verification [Id: {Id} | Processing Status: {Status}]",
                    entity.Id,
                    entity.ProcessingStatus
                );
                break;
            case VerificationStatus.Rejected:
                entity.ProcessingStatus = TournamentProcessingStatus.Done;
                break;
            case VerificationStatus.Verified:
                entity.ProcessingStatus = TournamentProcessingStatus.NeedsStatCalculation;
                break;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
