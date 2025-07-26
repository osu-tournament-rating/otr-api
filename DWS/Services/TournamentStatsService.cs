using Common.Enums.Verification;
using Database.Entities;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DWS.Services;

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
        Tournament? tournament = await tournamentsRepository.GetAsync(tournamentId, true);

        if (tournament == null)
        {
            logger.LogError("Tournament not found [Id: {Id}]", tournamentId);
            return false;
        }

        // Load additional relationships needed for stats processing
        await tournamentsRepository.LoadMatchesWithGamesAndScoresAsync(tournament);

        if (tournament.VerificationStatus is not VerificationStatus.Verified)
        {
            logger.LogError(
                "Stats processing was triggered for an unverified tournament, skipping [Id: {Id} | Verification Status: {Status}]",
                tournament.Id,
                tournament.VerificationStatus
            );

            return false;
        }

        // Process match stats for all verified matches
        foreach (Match match in tournament.Matches.Where(m => m.VerificationStatus == VerificationStatus.Verified))
        {
            await matchStatsService.ProcessMatchStatsAsync(match);
        }

        List<Match> verifiedMatches = [.. tournament.Matches
            .Where(m => m.VerificationStatus == VerificationStatus.Verified)];

        // Sanity check
        // If any processor data or stat objects are missing we cannot generate tournament stats
        foreach (Match match in verifiedMatches)
        {
            if (match.PlayerMatchStats.Count != 0
                && match.PlayerRatingAdjustments.Count != 0
                && match.Rosters.Count > 0)
            {
                // Check if the mismatch is due to restricted players
                var playersWithStats = match.PlayerMatchStats.Select(pms => pms.PlayerId).ToHashSet();
                var playersWithAdjustments = match.PlayerRatingAdjustments.Select(ra => ra.PlayerId).ToHashSet();
                var missingAdjustments = playersWithStats.Except(playersWithAdjustments).ToList();

                if (missingAdjustments.Count == 0)
                {
                    // Perfect match - all players have both stats and adjustments
                    continue;
                }

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

                continue;
            }

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

        // Create a dictionary to track existing PlayerTournamentStats by PlayerId
        var playerTournamentStatsDict = tournament.PlayerTournamentStats
            .ToDictionary(pts => pts.PlayerId);

        // Create or update a PlayerTournamentStats for each Player
        foreach (Player player in verifiedMatches
                     .SelectMany(m => m.PlayerMatchStats)
                     .Select(pms => pms.Player)
             .DistinctBy(p => p.Id))
        {
            List<PlayerMatchStats> matchStats = [.. verifiedMatches
                .SelectMany(m => m.PlayerMatchStats)
                .Where(pms => pms.Player.Id == player.Id)];

            RatingAdjustment[] matchAdjustments = [.. verifiedMatches
                .SelectMany(m => m.PlayerRatingAdjustments)
                .Where(ra => ra.Player.Id == player.Id)];

            // Skip players who don't have any rating adjustments
            if (matchAdjustments.Length == 0)
            {
                logger.LogDebug(
                    "Skipping tournament stats for player with no rating adjustments, likely restricted " +
                    "[Player Id: {PlayerId} | Tournament Id: {TournamentId}]",
                    player.Id,
                    tournament.Id
                );
                continue;
            }

            if (playerTournamentStatsDict.TryGetValue(player.Id, out PlayerTournamentStats? existingStats))
            {
                UpdatePlayerTournamentStats(existingStats, matchAdjustments, matchStats);
            }
            else
            {
                // Add a new PlayerTournamentStats
                var stats = new PlayerTournamentStats { PlayerId = player.Id, TournamentId = tournament.Id };
                UpdatePlayerTournamentStats(stats, matchAdjustments, matchStats);

                tournament.PlayerTournamentStats.Add(stats);
            }
        }

        tournament.LastProcessingDate = DateTime.UtcNow;

        await tournamentsRepository.UpdateAsync(tournament);

        return true;
    }

    private static void UpdatePlayerTournamentStats(PlayerTournamentStats existingStats, RatingAdjustment[] matchAdjustments,
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
