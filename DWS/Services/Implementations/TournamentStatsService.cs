using Common.Enums.Verification;
using Database.Entities;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;
using DWS.Services.Interfaces;

namespace DWS.Services.Implementations;

/// <summary>
/// Service for processing tournament statistics.
/// </summary>
public class TournamentStatsService(
    ILogger<TournamentStatsService> logger,
    ITournamentsRepository tournamentsRepository,
    IMatchesRepository matchesRepository,
    IMatchStatsService matchStatsService) : ITournamentStatsService
{
    /// <inheritdoc />
    public async Task<bool> ProcessTournamentStatsAsync(int tournamentId)
    {
        // Load and validate tournament
        Tournament? tournament = await LoadAndValidateTournamentAsync(tournamentId);
        if (tournament == null)
        {
            return false;
        }

        // Process match stats for all verified matches
        List<Match> verifiedMatches = await ProcessVerifiedMatchesAsync(tournament);

        // Validate all matches have required processor data
        if (!ValidateMatchProcessorDataAsync(verifiedMatches))
        {
            return false;
        }

        // Aggregate and update player statistics
        AggregatePlayerStatisticsAsync(tournament, verifiedMatches);

        // Update tournament processing date and persist
        tournament.LastProcessingDate = DateTime.UtcNow;
        await tournamentsRepository.UpdateAsync(tournament);

        logger.LogInformation(
            "Successfully processed tournament statistics [Id: {Id} | Verified Matches: {MatchCount} | Player Stats: {PlayerCount}]",
            tournament.Id,
            verifiedMatches.Count,
            tournament.PlayerTournamentStats.Count
        );

        return true;
    }

    /// <summary>
    /// Loads a tournament and validates it for stats processing.
    /// </summary>
    /// <param name="tournamentId">The tournament ID to load.</param>
    /// <returns>The loaded tournament if valid, null otherwise.</returns>
    private async Task<Tournament?> LoadAndValidateTournamentAsync(int tournamentId)
    {
        Tournament? tournament = await tournamentsRepository.GetAsync(tournamentId, true);

        if (tournament == null)
        {
            logger.LogError("Tournament not found [Id: {Id}]", tournamentId);
            return null;
        }

        // Load additional relationships needed for stats processing
        await tournamentsRepository.LoadMatchesWithGamesAndScoresAsync(tournament);

        if (tournament.VerificationStatus is VerificationStatus.Verified)
        {
            return tournament;
        }

        logger.LogError(
            "Stats processing was triggered for an unverified tournament, skipping [Id: {Id} | Verification Status: {Status}]",
            tournament.Id,
            tournament.VerificationStatus
        );
        return null;

    }

    /// <summary>
    /// Processes stats for all verified matches in a tournament.
    /// </summary>
    /// <param name="tournament">The tournament containing matches to process.</param>
    /// <returns>List of verified matches that were processed.</returns>
    private async Task<List<Match>> ProcessVerifiedMatchesAsync(Tournament tournament)
    {
        List<Match> verifiedMatches = [.. tournament.Matches
            .Where(m => m.VerificationStatus == VerificationStatus.Verified)];

        foreach (Match match in verifiedMatches)
        {
            await matchStatsService.ProcessMatchStatsAsync(match);
            // Save match stats changes to database
            await matchesRepository.UpdateAsync(match);
        }

        return verifiedMatches;
    }

    /// <summary>
    /// Validates that all matches have required processor data for stats generation.
    /// </summary>
    /// <param name="verifiedMatches">The matches to validate.</param>
    /// <returns>True if all matches have valid data, false otherwise.</returns>
    private bool ValidateMatchProcessorDataAsync(List<Match> verifiedMatches)
    {
        foreach (Match match in verifiedMatches.Where(match => !HasRequiredProcessorData(match)))
        {
            logger.LogWarning(
                "Verified match missing rating adjustments from otr-processor, aborting" +
                " [Match Id: {Id} | Rating Adjustments: {AdjustmentCount}]",
                match.Id,
                match.PlayerRatingAdjustments.Count
            );
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a match has been processed by the otr-processor.
    /// </summary>
    /// <param name="match">The match to check.</param>
    /// <returns>True if the processor has run (at least one rating adjustment exists), false otherwise.</returns>
    private static bool HasRequiredProcessorData(Match match)
    {
        return match.PlayerRatingAdjustments.Count != 0;
    }

    /// <summary>
    /// Aggregates player statistics across all verified matches and creates fresh tournament stats.
    /// </summary>
    /// <param name="tournament">The tournament to update.</param>
    /// <param name="verifiedMatches">The verified matches to aggregate stats from.</param>
    private void AggregatePlayerStatisticsAsync(Tournament tournament, List<Match> verifiedMatches)
    {
        // Clear existing stats to ensure fresh calculation
        tournament.PlayerTournamentStats.Clear();

        // Pre-aggregate data to avoid repeated LINQ queries
        var playerDataGroups = verifiedMatches
            .SelectMany(m => m.PlayerMatchStats.Select(pms => new { Match = m, Stats = pms }))
            .GroupBy(x => x.Stats.PlayerId)
            .Select(g => new
            {
                PlayerId = g.Key,
                g.First().Stats.Player,
                MatchStats = g.Select(x => x.Stats).ToList(),
                RatingAdjustments = verifiedMatches
                    .SelectMany(m => m.PlayerRatingAdjustments)
                    .Where(ra => ra.PlayerId == g.Key)
                    .ToArray()
            })
            .Where(x => x.RatingAdjustments.Length > 0) // Skip restricted players
            .ToList();

        foreach (var playerData in playerDataGroups)
        {
            if (playerData.RatingAdjustments.Length == 0)
            {
                logger.LogDebug(
                    "Skipping tournament stats for player with no rating adjustments, likely restricted " +
                    "[Player Id: {PlayerId} | Tournament Id: {TournamentId}]",
                    playerData.PlayerId,
                    tournament.Id
                );
                continue;
            }

            // Always create new PlayerTournamentStats
            var stats = new PlayerTournamentStats
            {
                PlayerId = playerData.PlayerId,
                TournamentId = tournament.Id
            };

            PopulatePlayerTournamentStats(stats, playerData.RatingAdjustments, playerData.MatchStats);
            tournament.PlayerTournamentStats.Add(stats);
        }
    }

    /// <summary>
    /// Populates a player's tournament statistics with aggregated match data.
    /// </summary>
    /// <param name="stats">The new stats object to populate.</param>
    /// <param name="matchAdjustments">Rating adjustments from matches.</param>
    /// <param name="matchStats">Match statistics for the player.</param>
    private static void PopulatePlayerTournamentStats(
        PlayerTournamentStats stats,
        RatingAdjustment[] matchAdjustments,
        List<PlayerMatchStats> matchStats)
    {
        stats.AverageRatingDelta = matchAdjustments.Average(ra => ra.RatingDelta);
        stats.AverageMatchCost = matchStats.Average(pms => pms.MatchCost);
        stats.AverageScore = (int)matchStats.Average(pms => pms.AverageScore);
        stats.AveragePlacement = matchStats.Average(pms => pms.AveragePlacement);
        stats.AverageAccuracy = matchStats.Average(pms => pms.AverageAccuracy);
        stats.MatchesPlayed = matchStats.Count;
        stats.MatchesWon = matchStats.Count(pms => pms.Won);
        stats.MatchesLost = matchStats.Count - stats.MatchesWon;
        stats.GamesPlayed = matchStats.Sum(pms => pms.GamesPlayed);
        stats.GamesWon = matchStats.Sum(pms => pms.GamesWon);
        stats.GamesLost = matchStats.Sum(pms => pms.GamesLost);
        stats.MatchWinRate = stats.MatchesWon / (double)matchStats.Count;
        stats.TeammateIds = [.. matchStats.SelectMany(pms => pms.TeammateIds).Distinct()];
    }
}
