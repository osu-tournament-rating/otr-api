using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;
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
            if (game.Rosters.Count != 0)
            {
                continue;
            }

            logger.LogWarning(
                "A verified game that has completed processing but does not contain any rosters, aborting " +
                "[Game Id: {Id}]",
                game.Id
            );

            return;
        }

        var currentStats = entity.PlayerMatchStats.ToDictionary(k => k.PlayerId, v => v);
        IEnumerable<PlayerMatchStats> generatedStats = GeneratePlayerMatchStats(verifiedGames, currentStats);

        entity.Rosters = RostersHelper.GenerateRosters(verifiedGames);
        entity.PlayerMatchStats = [.. generatedStats];
        entity.ProcessingStatus = MatchProcessingStatus.NeedsRatingProcessorData;
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
    private IEnumerable<PlayerMatchStats> GeneratePlayerMatchStats(IEnumerable<Game> games, Dictionary<int, PlayerMatchStats> existingStats)
    {
        var eGames = games.ToList();

        if (eGames.Count == 0)
        {
            throw new ArgumentException("The list of games must not be empty", nameof(games));
        }

        if (eGames.Any(g => g.Rosters.Count == 0))
        {
            logger.LogWarning("1 or more games have an empty roster!");
            return [];
        }

        if (eGames.Any(g => g.Match.Rosters.Count == 0))
        {
            logger.LogWarning("Rosters are empty for 1 or more matches in the provided games list!");
            return [];
        }

        // Calculate match costs
        IDictionary<int, double> matchCosts = MatchCostCalculator.CalculateOtrMatchCosts(eGames);

        // Precompute winning team for each game
        var gameWinningTeams = new Dictionary<Game, Team?>();
        foreach (Game game in eGames)
        {
            var teamScores = game.Scores
                .Where(s => s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done })
                .GroupBy(s => s.Team)
                .Select(g => new
                {
                    Team = g.Key,
                    TotalScore = g.Sum(s => s.Score)
                })
                .ToList();

            Team? winningTeam = teamScores
                .OrderByDescending(ts => ts.TotalScore)
                .FirstOrDefault()?.Team;

            gameWinningTeams[game] = winningTeam;
        }

        // Precompute max match score from match rosters
        ICollection<MatchRoster> matchRosters = eGames.First().Match.Rosters;
        var maxMatchScore = matchRosters.Max(r => r.Score);

        // Filter scores and group by player
        var playerScoreGroups = eGames
            .SelectMany(g => g.Scores.Where(s =>
                s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done })
                .GroupBy(s => s.Player.Id)
                .Select(group => (group.Key, group.ToList()))
            )
            .ToList();

        return playerScoreGroups
    .Select(group =>
    {
        (var playerId, List<GameScore> scores) = group;

        // Determine games the player participated in
        var playerGames = scores.Select(s => s.Game).Distinct().ToList();

        var gamesWon = 0;
        var gamesLost = 0;

        foreach (Game game in playerGames)
        {
            // Player's team in this specific game
            Team playerTeamInGame = scores.First(s => s.Game == game).Team;
            Team? winningTeam = gameWinningTeams[game];

            if (winningTeam == playerTeamInGame)
            {
                gamesWon++;
            }
            else if (winningTeam != null)
            {
                gamesLost++;
            }
        }

        // Determine match outcome for the player's team
        MatchRoster? playerMatchRoster = matchRosters.FirstOrDefault(r => r.Roster.Contains(playerId));
        var won = playerMatchRoster != null && playerMatchRoster.Score == maxMatchScore;

        // Determine teammate and opponent IDs based on match rosters
        List<int> teammateIds = playerMatchRoster?.Roster.Where(id => id != playerId).ToList() ?? [];
        var opponentIds = matchRosters
            .Where(r => r.Team != playerMatchRoster?.Team)
            .SelectMany(r => r.Roster)
            .Distinct()
            .Where(id => id != playerId)
            .ToList();

        // Create or update the player stats
        if (!existingStats.TryGetValue(playerId, out PlayerMatchStats? stat))
        {
            stat = new PlayerMatchStats
            {
                PlayerId = playerId
            };
            existingStats[playerId] = stat;
        }

        // Update the stats
        stat.MatchCost = matchCosts[playerId];
        stat.AverageScore = scores.Average(s => s.Score);
        stat.AveragePlacement = scores.Average(s => s.Placement);
        stat.AverageMisses = scores.Average(s => s.CountMiss);
        stat.AverageAccuracy = scores.Average(s => s.Accuracy);
        stat.GamesPlayed = scores.Count;
        stat.GamesWon = gamesWon;
        stat.GamesLost = gamesLost;
        stat.Won = won;
        stat.TeammateIds = [.. teammateIds];
        stat.OpponentIds = [.. opponentIds];

        return stat;
    });
    }
}
