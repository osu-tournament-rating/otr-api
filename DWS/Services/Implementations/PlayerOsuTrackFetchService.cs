using Common.Constants;
using Common.Enums;
using Common.Enums.Verification;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.OsuTrack;

namespace DWS.Services.Implementations;

public class PlayerOsuTrackFetchService(
    ILogger<PlayerOsuTrackFetchService> logger,
    OtrContext context,
    IOsuClient osuClient,
    IPlayersRepository playersRepository) : IPlayerOsuTrackFetchService
{
    public async Task<bool> FetchAndPersistAsync(long osuPlayerId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching osu!track data for player {OsuPlayerId}", osuPlayerId);

        Player? player = await context.Players
            .Include(p => p.RulesetData)
            .Include(p => p.MatchStats)
                .ThenInclude(ms => ms.Match)
            .FirstOrDefaultAsync(p => p.OsuId == osuPlayerId, cancellationToken);

        if (player is null)
        {
            logger.LogWarning("Player not found in database, aborting osu!track fetch [osu! ID: {OsuPlayerId}]", osuPlayerId);
            return false;
        }

        bool successfullyProcessedAnyRuleset = false;

        foreach (Ruleset ruleset in EnumConstants.FetchableRulesets)
        {
            try
            {
                bool processed = await ProcessRulesetDataAsync(player, ruleset, cancellationToken);
                if (processed)
                {
                    successfullyProcessedAnyRuleset = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing osu!track data for player [osu! ID: {OsuPlayerId} | Ruleset: {Ruleset}]",
                    osuPlayerId, ruleset);
            }
        }

        player.OsuTrackLastFetch = DateTime.UtcNow;
        if (successfullyProcessedAnyRuleset)
        {
            await playersRepository.UpdateAsync(player);

            logger.LogInformation("Successfully updated osu!track data for player {OsuPlayerId}", osuPlayerId);
            return true;
        }

        logger.LogWarning("No osu!track data was successfully processed for player {OsuPlayerId}", osuPlayerId);
        return false;
    }

    /// <summary>
    /// Processes osu!track data for a specific ruleset and updates the player's earliest rank data.
    /// </summary>
    /// <param name="player">The player to update.</param>
    /// <param name="ruleset">The ruleset to process.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>True if data was successfully processed, false otherwise.</returns>
    private async Task<bool> ProcessRulesetDataAsync(Player player, Ruleset ruleset, CancellationToken cancellationToken)
    {
        IEnumerable<UserStatUpdate>? statHistory = await osuClient.GetUserStatsHistoryAsync(
            player.OsuId,
            ruleset,
            cancellationToken: cancellationToken);

        if (statHistory is null)
        {
            logger.LogDebug("No osu!track data returned for player [osu! ID: {OsuPlayerId} | Ruleset: {Ruleset}]",
                player.OsuId, ruleset);
            return false;
        }

        var statUpdates = statHistory.ToList();
        if (statUpdates.Count == 0)
        {
            logger.LogTrace("Empty osu!track history for player [osu! ID: {OsuPlayerId} | Ruleset: {Ruleset}]",
                player.OsuId, ruleset);
            return false;
        }

        PlayerOsuRulesetData? rulesetData = player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset);
        if (rulesetData is null)
        {
            logger.LogDebug("Player has no data for ruleset, skipping [osu! ID: {OsuPlayerId} | Ruleset: {Ruleset}]",
                player.OsuId, ruleset);
            return false;
        }

        UserStatUpdate? earliestStat = await FindEarliestRelevantStatAsync(player, statUpdates, context, cancellationToken);
        if (earliestStat is null)
        {
            logger.LogTrace("No relevant osu!track stats found for player [osu! ID: {OsuPlayerId} | Ruleset: {Ruleset}]",
                player.OsuId, ruleset);
            return false;
        }

        bool updated = UpdateRulesetData(rulesetData, earliestStat);

        if (updated && ruleset == Ruleset.ManiaOther)
        {
            HandleManiaVariants(player, earliestStat);
        }

        return updated;
    }

    /// <summary>
    /// Finds the most relevant historical stat update for a player based on their tournament participation.
    /// </summary>
    /// <param name="player">The player to analyze.</param>
    /// <param name="statUpdates">The list of stat updates from osu!track.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The most relevant stat update, or null if none found.</returns>
    private static async Task<UserStatUpdate?> FindEarliestRelevantStatAsync(
        Player player,
        List<UserStatUpdate> statUpdates,
        OtrContext context,
        CancellationToken cancellationToken)
    {
        // First try to get the earliest match date from MatchStats if they exist
        DateTime? earliestMatchDate = player.MatchStats
            .DefaultIfEmpty()
            // If no MatchStats, query the database directly for verified matches
            .Min(pms => pms?.Match.StartTime) ?? await FindEarliestVerifiedMatchDateAsync(player.Id, context, cancellationToken);

        if (earliestMatchDate is not null)
        {
            return statUpdates
                .OrderBy(s => Math.Abs((s.Timestamp - earliestMatchDate.Value).Ticks))
                .FirstOrDefault();
        }

        return statUpdates.FirstOrDefault();
    }

    /// <summary>
    /// Finds the earliest verified match date for a player by querying the database directly.
    /// </summary>
    /// <param name="playerId">The player ID to search for.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The earliest verified match date, or null if none found.</returns>
    private static async Task<DateTime?> FindEarliestVerifiedMatchDateAsync(
        int playerId,
        OtrContext context,
        CancellationToken cancellationToken)
    {
        return await context.GameScores
            .Where(gs => gs.PlayerId == playerId)
            .Where(gs => gs.Game.Match.VerificationStatus == VerificationStatus.Verified)
            .Where(gs => gs.Game.Match.Tournament.VerificationStatus == VerificationStatus.Verified)
            .Select(gs => gs.Game.Match.StartTime)
            .MinAsync(cancellationToken);
    }

    /// <summary>
    /// Updates the ruleset data with historical rank information.
    /// </summary>
    /// <param name="rulesetData">The ruleset data to update.</param>
    /// <param name="statUpdate">The stat update containing historical data.</param>
    /// <returns>True if the data was updated, false if it was already present.</returns>
    private bool UpdateRulesetData(PlayerOsuRulesetData rulesetData, UserStatUpdate statUpdate)
    {
        if (rulesetData.EarliestGlobalRank is not null)
        {
            logger.LogDebug("Ruleset data already has earliest rank, skipping update [PlayerId: {PlayerId} | Ruleset: {Ruleset}]",
                rulesetData.PlayerId, rulesetData.Ruleset);
            return false;
        }

        rulesetData.EarliestGlobalRank = statUpdate.Rank;
        rulesetData.EarliestGlobalRankDate = statUpdate.Timestamp;

        logger.LogDebug("Updated earliest rank data [PlayerId: {PlayerId} | Ruleset: {Ruleset} | Rank: {Rank} | Date: {Date}]",
            rulesetData.PlayerId, rulesetData.Ruleset, statUpdate.Rank, statUpdate.Timestamp);

        return true;
    }

    /// <summary>
    /// Handles copying earliest rank data from ManiaOther to specific mania variants.
    /// </summary>
    /// <param name="player">The player to update.</param>
    /// <param name="statUpdate">The stat update containing historical data.</param>
    private void HandleManiaVariants(Player player, UserStatUpdate statUpdate)
    {
        Ruleset[] maniaVariants = [Ruleset.Mania4k, Ruleset.Mania7k];

        foreach (Ruleset variant in maniaVariants)
        {
            PlayerOsuRulesetData? variantData = player.RulesetData.FirstOrDefault(rd => rd.Ruleset == variant);

            if (variantData is null || variantData.EarliestGlobalRank is not null)
            {
                continue;
            }

            variantData.EarliestGlobalRank = statUpdate.Rank;
            variantData.EarliestGlobalRankDate = statUpdate.Timestamp;

            logger.LogDebug("Copied earliest rank data from ManiaOther to {Variant} [PlayerId: {PlayerId} | Rank: {Rank}]",
                variant, player.Id, statUpdate.Rank);
        }
    }
}
