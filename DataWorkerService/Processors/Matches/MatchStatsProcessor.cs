using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Utilities.Extensions;
using DataWorkerService.Processors.Resolvers.Interfaces;
using DataWorkerService.Utilities;

namespace DataWorkerService.Processors.Matches;

/// <summary>
/// Processor tasked with generating aggregate stats for a <see cref="Match"/>
/// </summary>
public class MatchStatsProcessor(
    ILogger<MatchStatsProcessor> logger,
    IGameProcessorResolver gameProcessorResolver
) : ProcessorBase<Match>(logger)
{
    protected override async Task OnProcessingAsync(Match entity, CancellationToken cancellationToken)
    {
        IProcessor<Game> gameStatsProcessor = gameProcessorResolver.GetStatsProcessor();
        foreach (Game game in entity.Games)
        {
            await gameStatsProcessor.ProcessAsync(game, cancellationToken);
        }

        if (
            entity.ProcessingStatus is not MatchProcessingStatus.NeedsStatCalculation
            || entity.Games.Any(g => g.ProcessingStatus is not GameProcessingStatus.Done)
            )
        {
            logger.LogDebug(
                "Match does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        IEnumerable<Game> verifiedGames = entity.Games.Where(g =>
            g.ProcessingStatus is GameProcessingStatus.Done
            && g.VerificationStatus is VerificationStatus.Verified
        )
        .ToList();

        if (verifiedGames.Any(g => g.WinRecord is null))
        {
            logger.LogDebug(
                "Match contains verified games with no win record [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        // Determine winning team by Games won
        Team winningTeam =
            verifiedGames.Count(g => g.WinRecord!.WinnerTeam is Team.Blue) >
            verifiedGames.Count(g => g.WinRecord!.WinnerTeam is Team.Red)
                ? Team.Blue
                : Team.Red;

        // Create the MatchWinRecord
        entity.WinRecord = new MatchWinRecord
        {
            WinnerRoster = verifiedGames
                .Where(g => g.WinRecord!.WinnerTeam == winningTeam)
                .SelectMany(g => g.WinRecord!.WinnerRoster)
                .Distinct()
                .ToArray(),
            LoserRoster = verifiedGames
                .Where(g => g.WinRecord!.WinnerTeam == winningTeam.OppositeTeam())
                .SelectMany(g => g.WinRecord!.WinnerRoster)
                .Distinct()
                .ToArray(),
            WinnerTeam = winningTeam,
            LoserTeam = winningTeam.OppositeTeam(),
            WinnerScore = verifiedGames.Count(g => g.WinRecord!.WinnerTeam == winningTeam),
            LoserScore = verifiedGames.Count(g => g.WinRecord!.WinnerTeam == winningTeam.OppositeTeam())
        };

        IDictionary<int, double> matchCosts = MatchCostCalculator.CalculateOtrMatchCosts(verifiedGames);

        // Create a PlayerMatchStats for each Player
        foreach (IGrouping<int, GameScore> group in verifiedGames
                     .SelectMany(g => g.Scores.Where(s =>
                         s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done }
                         )
                     )
                     .GroupBy(s => s.Player.Id)
                 )
        {
            var playerId = group.Key;
            IEnumerable<GameScore> playerScores = group.ToList();

            entity.PlayerMatchStats.Add(new PlayerMatchStats
            {
                MatchCost = matchCosts[playerId],
                AverageScore = playerScores.Average(s => s.Score),
                AveragePlacement = playerScores.Average(s => s.Placement),
                AverageMisses = playerScores.Average(s => s.CountMiss),
                AverageAccuracy = playerScores.Average(s => s.Accuracy),
                GamesPlayed = playerScores.Count(),
                GamesWon = verifiedGames.Count(g => g.WinRecord!.WinnerRoster.Contains(playerId)),
                GamesLost = verifiedGames.Count(g => g.WinRecord!.LoserRoster.Contains(playerId)),
                Won = entity.WinRecord!.WinnerRoster.Contains(playerId),
                TeammateIds = verifiedGames
                    .SelectMany(g =>
                        g.WinRecord!.WinnerRoster.Contains(playerId)
                            ? g.WinRecord!.WinnerRoster
                            : g.WinRecord!.LoserRoster
                    ).Distinct().Where(id => id != playerId).ToArray(),
                OpponentIds = verifiedGames
                    .SelectMany(g =>
                        g.WinRecord!.WinnerRoster.Contains(playerId)
                            ? g.WinRecord!.LoserRoster
                            : g.WinRecord!.WinnerRoster
                    ).Distinct().Where(id => id != playerId).ToArray(),
                PlayerId = playerId
            });
        }

        entity.ProcessingStatus += 1;
    }
}
