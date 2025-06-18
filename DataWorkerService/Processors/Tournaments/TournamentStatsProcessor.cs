using Common.Enums.Verification;
using Database.Entities;
using Database.Entities.Processor;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Tournaments;

/// <summary>
/// Processor tasked with generating aggregate stats for a <see cref="Tournament"/>
/// </summary>
public class TournamentStatsProcessor(
    ILogger<TournamentStatsProcessor> logger,
    IMatchProcessorResolver matchProcessorResolver
) : ProcessorBase<Tournament>(logger)
{
    protected override async Task OnProcessingAsync(Tournament entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not TournamentProcessingStatus.NeedsStatCalculation)
        {
            logger.LogDebug(
                "Tournament does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        if (!entity.Matches.All(m => m.ProcessingStatus is MatchProcessingStatus.Done))
        {
            IProcessor<Match> matchStatsProcessor = matchProcessorResolver.GetStatsProcessor();
            foreach (Match match in entity.Matches)
            {
                await matchStatsProcessor.ProcessAsync(match, cancellationToken);
            }

            if (!entity.Matches.All(m => m.ProcessingStatus is MatchProcessingStatus.Done))
            {
                logger.LogDebug(
                    "Tournament's matches are still awaiting stat generation [Id: {Id} | Processing Status: {Status}]",
                    entity.Id,
                    entity.ProcessingStatus
                );

                return;
            }
        }

        List<Match> verifiedMatches = [.. entity.Matches
            .Where(m => m is
            { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: MatchProcessingStatus.Done })];

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
                "A verified match that has completed processing contains unexpected stats, aborting" +
                " [Match Id: {Id} | {Stat1}: {Stat1C} | {Stat2}: {Stat2C} | Has WinRecord: {HasWinRecord}]",
                match.Id,
                nameof(Match.PlayerMatchStats),
                match.PlayerMatchStats.Count,
                nameof(match.PlayerRatingAdjustments),
                match.PlayerRatingAdjustments.Count,
                match.Rosters.Count > 0
            );

            return;
        }

        // Create a dictionary to track existing PlayerTournamentStats by PlayerId
        var playerTournamentStatsDict = entity.PlayerTournamentStats
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
                    entity.Id
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
                var stats = new PlayerTournamentStats { PlayerId = player.Id, TournamentId = entity.Id };
                UpdatePlayerTournamentStats(stats, matchAdjustments, matchStats);

                entity.PlayerTournamentStats.Add(stats);
            }
        }

        entity.ProcessingStatus = TournamentProcessingStatus.Done;
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
