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
                "Verified match missing required data for tournament stats generation (may not have been processed by otr-processor yet), aborting" +
                " [Match Id: {Id} | {Stat1}: {Stat1C} | {Stat2}: {Stat2C} | Has WinRecord: {HasWinRecord}]",
                match.Id,
                nameof(Match.PlayerMatchStats),
                match.PlayerMatchStats.Count,
                nameof(match.PlayerRatingAdjustments),
                match.PlayerRatingAdjustments.Count,
                match.Rosters.Count > 0
            );
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a match has the required processor data, accounting for restricted players.
    /// </summary>
    /// <param name="match">The match to check.</param>
    /// <returns>True if the match has valid processor data.</returns>
    private bool HasRequiredProcessorData(Match match)
    {
        // Match must have some data
        if (match.PlayerMatchStats.Count == 0 || match.Rosters.Count == 0)
        {
            return false;
        }

        // If no rating adjustments at all, it hasn't been processed
        if (match.PlayerRatingAdjustments.Count == 0)
        {
            return false;
        }

        // Check for mismatches due to restricted players
        var playersWithStats = match.PlayerMatchStats.Select(pms => pms.PlayerId).ToHashSet();
        var playersWithAdjustments = match.PlayerRatingAdjustments.Select(ra => ra.PlayerId).ToHashSet();
        var missingAdjustments = playersWithStats.Except(playersWithAdjustments).ToList();

        if (missingAdjustments.Count > 0)
        {
            // Log the mismatch for investigation but allow processing to continue
            // This handles cases where players became restricted after match completion
            logger.LogInformation(
                "Match has players with stats but no rating adjustments, likely due to player restrictions " +
                "[Match Id: {Id} | Players missing adjustments: {MissingPlayers} | Stats: {StatsCount} | Adjustments: {AdjustmentsCount}]",
                match.Id,
                string.Join(", ", missingAdjustments),
                match.PlayerMatchStats.Count,
                match.PlayerRatingAdjustments.Count
            );
        }

        return true;
    }

    /// <summary>
    /// Aggregates player statistics across all verified matches and updates tournament stats.
    /// </summary>
    /// <param name="tournament">The tournament to update.</param>
    /// <param name="verifiedMatches">The verified matches to aggregate stats from.</param>
    private void AggregatePlayerStatisticsAsync(Tournament tournament, List<Match> verifiedMatches)
    {
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

        // Create a dictionary to track existing PlayerTournamentStats
        var playerTournamentStatsDict = tournament.PlayerTournamentStats
            .ToDictionary(pts => pts.PlayerId);

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

            if (playerTournamentStatsDict.TryGetValue(playerData.PlayerId, out PlayerTournamentStats? existingStats))
            {
                UpdatePlayerTournamentStats(existingStats, playerData.RatingAdjustments, playerData.MatchStats);
            }
            else
            {
                // Add a new PlayerTournamentStats
                var stats = new PlayerTournamentStats
                {
                    PlayerId = playerData.PlayerId,
                    TournamentId = tournament.Id
                };
                UpdatePlayerTournamentStats(stats, playerData.RatingAdjustments, playerData.MatchStats);
                tournament.PlayerTournamentStats.Add(stats);
            }
        }
    }

    /// <summary>
    /// Updates a player's tournament statistics with aggregated match data.
    /// </summary>
    /// <param name="existingStats">The stats object to update.</param>
    /// <param name="matchAdjustments">Rating adjustments from matches.</param>
    /// <param name="matchStats">Match statistics for the player.</param>
    private static void UpdatePlayerTournamentStats(
        PlayerTournamentStats existingStats,
        RatingAdjustment[] matchAdjustments,
        List<PlayerMatchStats> matchStats)
    {
        existingStats.AverageRatingDelta = matchAdjustments.Average(ra => ra.RatingDelta);
        existingStats.AverageMatchCost = matchStats.Average(pms => pms.MatchCost);
        existingStats.AverageScore = (int)matchStats.Average(pms => pms.AverageScore);
        existingStats.AveragePlacement = matchStats.Average(pms => pms.AveragePlacement);
        existingStats.AverageAccuracy = matchStats.Average(pms => pms.AverageAccuracy);
        existingStats.MatchesPlayed = matchStats.Count;
        existingStats.MatchesWon = matchStats.Count(pms => pms.Won);
        existingStats.MatchesLost = matchStats.Count - existingStats.MatchesWon;
        existingStats.GamesPlayed = matchStats.Sum(pms => pms.GamesPlayed);
        existingStats.GamesWon = matchStats.Sum(pms => pms.GamesWon);
        existingStats.GamesLost = matchStats.Sum(pms => pms.GamesLost);
        existingStats.MatchWinRate = existingStats.MatchesWon / (double)matchStats.Count;
        existingStats.TeammateIds = [.. matchStats.SelectMany(pms => pms.TeammateIds).Distinct()];
    }
}
