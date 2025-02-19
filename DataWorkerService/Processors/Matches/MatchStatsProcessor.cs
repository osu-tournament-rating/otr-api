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
        if (entity.ProcessingStatus is not MatchProcessingStatus.NeedsStatCalculation)
        {
            logger.LogDebug(
                "Match does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        if (!entity.Games.All(g => g.ProcessingStatus is GameProcessingStatus.Done))
        {
            IProcessor<Game> gameStatsProcessor = gameProcessorResolver.GetStatsProcessor();
            foreach (Game game in entity.Games)
            {
                await gameStatsProcessor.ProcessAsync(game, cancellationToken);
            }

            if (!entity.Games.All(g => g.ProcessingStatus is GameProcessingStatus.Done))
            {
                logger.LogDebug(
                    "Match's games are still awaiting stat generation [Id: {Id} | Processing Status: {Status}]",
                    entity.Id,
                    entity.ProcessingStatus
                );

                return;
            }
        }

        IEnumerable<Game> verifiedGames = [.. entity.Games.Where(g => g is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: GameProcessingStatus.Done })];

        // Sanity check
        foreach (Game game in verifiedGames)
        {
            if (game.WinRecord is not null)
            {
                continue;
            }

            logger.LogWarning(
                "A verified game that has completed processing contains unexpected stats, aborting" +
                "[Game Id: {Id} | Has WinRecord: {HasWinRecord}]",
                game.Id,
                game.WinRecord is not null
            );

            return;
        }

        entity.WinRecord = GenerateWinRecord(verifiedGames);
        entity.PlayerMatchStats = [.. GeneratePlayerMatchStats(verifiedGames)];

        entity.ProcessingStatus = MatchProcessingStatus.NeedsRatingProcessorData;
    }

    /// <summary>
    /// Generates a <see cref="MatchWinRecord"/> for a given list of <see cref="Game"/>s
    /// </summary>
    /// <param name="games">List of <see cref="Game"/>s</param>
    /// <exception cref="ArgumentException">
    /// If any given <see cref="Game"/>s contains a null <see cref="Game.WinRecord"/>
    /// </exception>
    public static MatchWinRecord GenerateWinRecord(IEnumerable<Game> games)
    {
        var eGames = games.ToList();

        if (eGames.Any(g => g.WinRecord is null))
        {
            throw new ArgumentException(
                $"The property {nameof(Game.WinRecord)} must not be null in any object in the list",
                nameof(games)
            );
        }

        Team winningTeam = eGames
            .Select(g => g.WinRecord)
            .GroupBy(gwr => gwr!.WinnerTeam)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .First();

        Team losingTeam = winningTeam.OppositeTeam();

        return new MatchWinRecord
        {
            WinnerTeam = winningTeam,
            LoserTeam = losingTeam,
            WinnerRoster = [.. eGames
                .Select(g => g.WinRecord)
                .SelectMany(gwr => gwr!.WinnerTeam == winningTeam ? gwr.WinnerRoster : gwr.LoserRoster)
                .Distinct()],
            LoserRoster = [.. eGames
                .Select(g => g.WinRecord)
                .SelectMany(gwr => gwr!.WinnerTeam == losingTeam ? gwr.WinnerRoster : gwr.LoserRoster)
                .Distinct()],
            WinnerScore = eGames.Count(g => g.WinRecord!.WinnerTeam == winningTeam),
            LoserScore = eGames.Count(g => g.WinRecord!.WinnerTeam == losingTeam),
        };
    }

    /// <summary>
    /// Generates a list of <see cref="PlayerMatchStats"/> for a given list of <see cref="Game"/>s,
    /// one per unique <see cref="Player"/>
    /// </summary>
    /// <param name="games">List of <see cref="Game"/>s</param>
    /// <exception cref="ArgumentException">
    /// If the given list of <see cref="Game"/>s is empty
    /// <br/>If any given <see cref="Game"/>s contains a null <see cref="Game.WinRecord"/>
    /// <br/>If the parent <see cref="Match"/> contains a null <see cref="Match.WinRecord"/>
    /// </exception>
    public static IEnumerable<PlayerMatchStats> GeneratePlayerMatchStats(IEnumerable<Game> games)
    {
        var eGames = games.ToList();

        if (eGames.Count == 0)
        {
            throw new ArgumentException("The list of games must not be empty", nameof(games));
        }

        if (eGames.Any(g => g.WinRecord is null))
        {
            throw new ArgumentException(
                $"The property {nameof(Game.WinRecord)} must not be null in any object in the list",
                nameof(games)
            );
        }

        if (eGames.Any(g => g.Match.WinRecord is null))
        {
            throw new ArgumentException(
                $"The property {nameof(Game.Match.WinRecord)} must not be null in any object in the list",
                nameof(games)
            );
        }

        // Calculate match costs
        IDictionary<int, double> matchCosts = MatchCostCalculator.CalculateOtrMatchCosts(eGames);

        // Filter scores and group by player
        var playerScoreGroups = eGames
            .SelectMany(g => g.Scores.Where(s =>
                    s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done }
                )
            )
            .GroupBy(s => s.Player.Id)
            .Select(group => (group.Key, group.ToList()))
            .ToList();

        return playerScoreGroups
            .Select(group =>
            {
                (var playerId, List<GameScore> scores) = group;

                return new PlayerMatchStats
                {
                    MatchCost = matchCosts[playerId],
                    AverageScore = scores.Average(s => s.Score),
                    AveragePlacement = scores.Average(s => s.Placement),
                    AverageMisses = scores.Average(s => s.CountMiss),
                    AverageAccuracy = scores.Average(s => s.Accuracy),
                    GamesPlayed = scores.Count,
                    GamesWon = scores.Count(s => s.Game.WinRecord!.WinnerRoster.Contains(playerId)),
                    GamesLost = scores.Count(s => s.Game.WinRecord!.LoserRoster.Contains(playerId)),
                    Won = scores.First().Game.Match.WinRecord!.WinnerRoster.Contains(playerId),
                    // Reusing score groupings to ensure score filtering
                    TeammateIds = [.. playerScoreGroups
                        .Where(g => g.Item2.All(s => s.Team == scores.First().Team))
                        .Select(g => g.Key)
                        .Where(id => id != playerId)],
                    OpponentIds = [.. playerScoreGroups
                        .Where(g => g.Item2.All(s => s.Team != scores.First().Team))
                        .Select(g => g.Key)
                        .Where(id => id != playerId)],
                    PlayerId = playerId
                };
            });
    }
}
