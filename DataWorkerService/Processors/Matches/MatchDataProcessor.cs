using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using DataWorkerService.Services.Interfaces;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Multiplayer;
using GameScore = Database.Entities.GameScore;

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

            // Match skips automation checks and proceeds to manual review
            entity.ProcessingStatus = MatchProcessingStatus.NeedsVerification;

            return;
        }

        // Parse api response
        logger.LogDebug(
            "Parsing match [Id: {Id} | osu! Id: {OsuId}]",
            entity.Id,
            entity.OsuId
        );

        await parserService.ParseMatchAsync(entity, response);

        // Set mania variant ruleset
        if (entity.Tournament.Ruleset is Ruleset.Mania4k or Ruleset.Mania7k)
        {
            foreach (Game game in entity.Games.Where(g => g.Ruleset is Ruleset.ManiaOther))
            {
                game.Ruleset = entity.Tournament.Ruleset;

                foreach (GameScore score in game.Scores.Where(s => s.Ruleset is Ruleset.ManiaOther))
                {
                    score.Ruleset = entity.Tournament.Ruleset;
                }
            }
        }

        entity.ProcessingStatus = MatchProcessingStatus.NeedsAutomationChecks;
    }
}
