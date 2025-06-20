﻿using Common.Enums;
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
    public static ICollection<MatchRoster> GenerateRosters(IEnumerable<Game> games)
    {
        var eGames = games.ToList();

        if (eGames.Count == 0)
        {
            return [];
        }

        Dictionary<Team, int> pointsEarned = [];
        var allGameRosters = new List<GameRoster>();

        foreach (Game game in eGames)
        {
            // Use existing rosters if available, otherwise generate from scores
            ICollection<GameRoster> gameRosters;
            if (game.Rosters.Count > 0)
            {
                gameRosters = game.Rosters;
            }
            else
            {
                // Generate rosters from verified scores without modifying the game entity
                IEnumerable<GameScore> verifiedScores = game.Scores
                    .Where(s => s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done });
                gameRosters = GenerateRosters(verifiedScores);
            }

            // Skip games with no rosters
            if (gameRosters.Count == 0)
            {
                continue;
            }

            allGameRosters.AddRange(gameRosters);

            // Determine the winning team for this game using verified scores
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

            // If a winning team is found, increment their points
            if (winningTeam == null)
            {
                continue;
            }

            pointsEarned.TryAdd(winningTeam.Value, 0);
            pointsEarned[winningTeam.Value]++;
        }

        // Build match rosters from all game rosters
        List<MatchRoster> matchRosters = [.. allGameRosters
            .GroupBy(gr => gr.Team)
            .Select(group => new MatchRoster
            {
                Team = group.Key,
                Roster = [.. group.SelectMany(r => r.Roster).Distinct()],
                Score = pointsEarned.GetValueOrDefault(group.Key, 0)
            })];

        return matchRosters;
    }
}
