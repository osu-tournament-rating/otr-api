using Common.Enums.Enums.Verification;
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
                && match.PlayerMatchStats.Count == match.PlayerRatingAdjustments.Count
                && match.Rosters.Count > 0
               )
            {
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

            IEnumerable<RatingAdjustment> matchAdjustments = [.. verifiedMatches
                .SelectMany(m => m.PlayerRatingAdjustments)
                .Where(ra => ra.Player.Id == player.Id)];

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

    private static void UpdatePlayerTournamentStats(PlayerTournamentStats existingStats, IEnumerable<RatingAdjustment> matchAdjustments,
        List<PlayerMatchStats> matchStats)
    {
        existingStats.AverageRatingDelta = matchAdjustments.Average(ra => ra.RatingDelta);
        existingStats.AverageMatchCost = matchStats.Average(pms => pms.MatchCost);
        existingStats.AverageScore = (int)matchStats.Average(pms => pms.AverageScore);
        existingStats.AveragePlacement = matchStats.Average(pms => pms.AveragePlacement);
        existingStats.AverageAccuracy = matchStats.Average(pms => pms.AverageAccuracy);
        existingStats.MatchesPlayed = matchStats.Count;
        existingStats.MatchesWon = matchStats.Count(pms => pms.Won);
        existingStats.MatchesLost = matchStats.Count(pms => !pms.Won);
        existingStats.GamesPlayed = matchStats.Sum(pms => pms.GamesPlayed);
        existingStats.GamesWon = matchStats.Sum(pms => pms.GamesWon);
        existingStats.GamesLost = matchStats.Sum(pms => pms.GamesLost);
        existingStats.TeammateIds = [.. matchStats.SelectMany(pms => pms.TeammateIds).Distinct()];
    }
}
