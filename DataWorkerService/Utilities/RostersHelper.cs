using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.Utilities;

public static class RostersHelper
{
    /// <summary>
    /// Generates a <see cref="GameRoster"/> for a given list of <see cref="GameScore"/>s
    /// </summary>
    /// <param name="scores">List of <see cref="GameScore"/>s</param>
    public static ICollection<GameRoster> GenerateRosters(IEnumerable<GameScore> scores)
    {
        var eScores = scores.ToList();

        if (eScores.Count == 0)
        {
            return [];
        }

        // Sanity check for different game IDs
        if (eScores.Select(gs => gs.GameId).Distinct().Count() > 1)
        {
            throw new InvalidOperationException("All scores must belong to the same game id");
        }

        var gameRosters = eScores
            .GroupBy(gs => gs.Team) // Group by Team only
            .Select(group => new GameRoster
            {
                GameId = group.First().GameId, // Use the GameId from the first score in the group
                Team = group.Key,
                Roster = [.. group.Select(gs => gs.PlayerId).Distinct()], // Ensure unique PlayerIds
                Score = group.Sum(gs => gs.Score)
            })
            .ToList();

        return gameRosters;
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

        List<MatchRoster> rosters = [.. eGames
            .SelectMany(g => g.Rosters)
            .GroupBy(
                gr => gr.Team,
                (team, rosters) => new MatchRoster
                {
                    Team = team,
                    Roster = [.. rosters.SelectMany(r => r.Roster).Distinct()],
                    Score = pointsEarned.GetValueOrDefault(team, 0)
                })];

        return rosters;
    }
}
