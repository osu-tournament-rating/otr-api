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
    IMatchProcessorResolver matchProcessorResolver
) : ProcessorBase<Tournament>(logger)
{
    protected override async Task OnProcessingAsync(Tournament entity, CancellationToken cancellationToken)
    {
        IProcessor<Match> matchStatsProcessor = matchProcessorResolver.GetStatsProcessor();
        foreach (Match match in entity.Matches)
        {
            await matchStatsProcessor.ProcessAsync(match, cancellationToken);
        }

        if (
            entity.ProcessingStatus is not TournamentProcessingStatus.NeedsStatCalculation
            || entity.Matches.Any(m => m.ProcessingStatus is not MatchProcessingStatus.Done)
            )
        {
            logger.LogDebug(
                "Tournament does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        IEnumerable<Match> verifiedMatches = entity.Matches.Where(m =>
            m is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: MatchProcessingStatus.Done }
        )
        .ToList();

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
    }
}
