using Database;
using Database.Entities;
using Database.Entities.Processor;
using Database.Enums.Verification;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Tournaments;

/// <summary>
/// Processor tasked with generating aggregate stats for a <see cref="Tournament"/>
/// </summary>
public class TournamentStatsProcessor(
    ILogger<TournamentStatsProcessor> logger,
    IMatchProcessorResolver matchProcessorResolver,
    OtrContext context
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

        IEnumerable<Match> verifiedMatches = entity.Matches
            .Where(m => m is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: MatchProcessingStatus.Done })
            .ToList();

        // Sanity check
        // If any processor data or stat objects are missing we cannot generate tournament stats
        foreach (Match match in verifiedMatches)
        {
            if (match.PlayerMatchStats.Count != 0
                && match.PlayerRatingAdjustments.Count != 0
                && match.PlayerMatchStats.Count == match.PlayerRatingAdjustments.Count
                && match.WinRecord is not null
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
                match.WinRecord is not null
            );

            return;
        }

        // Create a PlayerTournamentStats for each Player
        foreach (Player player in verifiedMatches
                     .SelectMany(m => m.PlayerMatchStats)
                     .Select(pms => pms.Player)
                     .DistinctBy(p => p.Id)
                )
        {
            IEnumerable<PlayerMatchStats> matchStats = verifiedMatches
                .SelectMany(m => m.PlayerMatchStats)
                .Where(pms => pms.Player.Id == player.Id)
                .ToList();

            IEnumerable<RatingAdjustment> matchAdjustments = verifiedMatches
                .SelectMany(m => m.PlayerRatingAdjustments)
                .Where(ra => ra.Player.Id == player.Id)
                .ToList();

            entity.PlayerTournamentStats.Add(new PlayerTournamentStats
            {
                AverageRatingDelta = matchAdjustments.Average(ra => ra.RatingDelta),
                AverageMatchCost = matchStats.Average(pms => pms.MatchCost),
                AverageScore = (int)matchStats.Average(pms => pms.AverageScore),
                AveragePlacement = matchStats.Average(pms => pms.AveragePlacement),
                AverageAccuracy = matchStats.Average(pms => pms.AverageAccuracy),
                MatchesPlayed = matchStats.Count(),
                MatchesWon = matchStats.Count(pms => pms.Won),
                MatchesLost = matchStats.Count(pms => !pms.Won),
                GamesPlayed = matchStats.Sum(pms => pms.GamesPlayed),
                GamesWon = matchStats.Sum(pms => pms.GamesWon),
                GamesLost = matchStats.Sum(pms => pms.GamesLost),
                TeammateIds = matchStats.SelectMany(pms => pms.TeammateIds).Distinct().ToArray(),
                PlayerId = player.Id,
                TournamentId = entity.Id
            });
        }

        entity.ProcessingStatus = TournamentProcessingStatus.Done;

        await context.SaveChangesAsync(cancellationToken);
    }
}
