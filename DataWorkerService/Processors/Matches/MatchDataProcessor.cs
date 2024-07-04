using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.Services.Interfaces;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Multiplayer;

namespace DataWorkerService.Processors.Matches;

/// <summary>
/// Processor tasked with fetching data from outside sources for a <see cref="Match"/>
/// </summary>
public class MatchDataProcessor(
    ILogger<MatchDataProcessor> logger,
    IOsuClient osuClient,
    IOsuApiDataParserService parserService
) : ProcessorBase<Match>(logger)
{
    protected override async Task OnProcessingAsync(Match entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not MatchProcessingStatus.NeedsData)
        {
            logger.LogDebug(
                "Match does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        // Fetch the match from the osu! API
        MultiplayerMatch? response = await osuClient.GetMatchAsync(entity.OsuId, cancellationToken);

        // Handle "dead" mp links
        if (response is null)
        {
            logger.LogDebug(
                "Unable to fetch match from the osu! API. Match will be Pre-Rejected [Id: {Id} | osu! Id: {OsuId}]",
                entity.Id,
                entity.OsuId
            );

            entity.VerificationStatus = VerificationStatus.PreRejected;
            entity.RejectionReason |= MatchRejectionReason.NoData;

            entity.ProcessingStatus = MatchProcessingStatus.Done;

            return;
        }

        // Parse api response
        logger.LogDebug(
            "Parsing match [Id: {Id} | osu! Id: {OsuId}]",
            entity.Id,
            entity.OsuId
        );

        await parserService.ParseMatchAsync(entity, response);

        entity.ProcessingStatus += 1;
    }
}
