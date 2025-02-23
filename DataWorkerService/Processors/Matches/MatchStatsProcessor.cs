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

        List<Game> verifiedGames = [.. entity.Games.Where(g => g is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: GameProcessingStatus.Done })];

        // Sanity check
        foreach (Game game in verifiedGames)
        {
            if (game.Rosters is not null)
            {
                continue;
            }

            logger.LogWarning(
                "A verified game that has completed processing contains unexpected stats, aborting" +
                "[Game Id: {Id} | Has WinRecord: {HasWinRecord}]",
                game.Id,
                game.Rosters is not null
            );

            return;
        }

        entity.Rosters = GenerateRosters(verifiedGames);
        entity.PlayerMatchStats = [.. GeneratePlayerMatchStats(verifiedGames)];
        entity.ProcessingStatus = MatchProcessingStatus.NeedsRatingProcessorData;
    }

    /// <summary>
    /// Generates a <see cref="MatchRoster"/> for a given list of <see cref="Game"/>s
    /// </summary>
    /// <param name="games">List of <see cref="Game"/>s</param>
    /// <exception cref="ArgumentException">
    /// If any given <see cref="Game"/>s contains a null <see cref="Game.Rosters"/>
    /// </exception>
    public static ICollection<MatchRoster> GenerateRosters(IEnumerable<Game> games)
    {
        var eGames = games.ToList();

        if (eGames.Any(g => g.Rosters.Count == 0))
        {
            throw new ArgumentException(
                $"The property {nameof(Game.Rosters)} must not be empty for any {nameof(Game)} in this collection",
                nameof(games)
            );
        }

        IEnumerable<IGrouping<Team, GameRoster>> teamRosters = eGames
            .SelectMany(g => g.Rosters)
            .GroupBy(gr => gr.Team);

        Dictionary<Team, int> pointsEarned = [];

        foreach (Game? game in eGames)
        {
            // Group the game by Team, then sum the value of all GameScores.
            var teamScores = game.Scores
                .Where(s => s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done })
                .GroupBy(s => s.Team)
                .Select(g => new
                {
                    Team = g.Key,
                    TotalScore = g.Sum(s => s.Score)
                })
                .ToList();

            // Determine the winning team for this game.
            Team? winningTeam = teamScores
                .OrderByDescending(ts => ts.TotalScore)
                .FirstOrDefault()?.Team;

            // If a winning team is found, increment their points.
            if (winningTeam != null)
            {
                pointsEarned.TryAdd(winningTeam.Value, 0);
                pointsEarned[winningTeam.Value]++;
            }
        }

        var rosters = new List<MatchRoster>();

        foreach (IGrouping<Team, GameRoster?> gameRoster in teamRosters)
        {
            rosters.Add(new MatchRoster
            {
                Team = gameRoster.Key,
                Roster = [.. gameRoster.SelectMany(gr => gr!.Roster).Distinct()],
                Score = pointsEarned.GetValueOrDefault(gameRoster.Key, 0)
            });
        }

        return rosters;
    }

    /// <summary>
    /// Generates a list of <see cref="PlayerMatchStats"/> for a given list of <see cref="Game"/>s,
    /// one per unique <see cref="Player"/>
    /// </summary>
    /// <param name="games">List of <see cref="Game"/>s</param>
    /// <exception cref="ArgumentException">
    /// If the given list of <see cref="Game"/>s is empty
    /// <br/>If any given <see cref="Game"/>s contains a null <see cref="Game.Rosters"/>
    /// <br/>If the parent <see cref="Match"/> contains a null <see cref="Match.Rosters"/>
    /// </exception>
    public static IEnumerable<PlayerMatchStats> GeneratePlayerMatchStats(IEnumerable<Game> games)
    {
        var eGames = games.ToList();

        if (eGames.Count == 0)
        {
            throw new ArgumentException("The list of games must not be empty", nameof(games));
        }

        if (eGames.Any(g => g.Rosters.Count == 0))
        {
            throw new ArgumentException(
                $"The property {nameof(Game.Rosters)} must not be empty",
                nameof(games)
            );
        }

        if (eGames.Any(g => g.Match.Rosters.Count == 0))
        {
            throw new ArgumentException(
                $"The property {nameof(Game.Match.Rosters)} must not be empty",
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

                // Get the first game's roster for the player's team
                Game firstGame = scores.First().Game;
                Team playerTeam = scores.First().Team;
                var playerRoster = firstGame.Rosters.FirstOrDefault(r => r.Team == playerTeam)?.Roster ?? [];

                // Get the match roster for the player's team
                var matchRoster = firstGame.Match.Rosters.FirstOrDefault(r => r.Team == playerTeam)?.Roster ?? [];

                return new PlayerMatchStats
                {
                    MatchCost = matchCosts[playerId],
                    AverageScore = scores.Average(s => s.Score),
                    AveragePlacement = scores.Average(s => s.Placement),
                    AverageMisses = scores.Average(s => s.CountMiss),
                    AverageAccuracy = scores.Average(s => s.Accuracy),
                    GamesPlayed = scores.Count,
                    GamesWon = scores.Count(s => playerRoster.Contains(playerId)),
                    GamesLost = scores.Count(s => matchRoster.Contains(playerId) && s.Game.Rosters.Any(r => r.Team != playerTeam && r.Roster.Contains(playerId))),
                    Won = matchRoster.Contains(playerId),
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
