using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Utilities.Extensions;

namespace DataWorkerService.Processors.Games;

/// <summary>
/// Processor tasked with generating aggregate stats for a <see cref="Game"/>
/// </summary>
public class GameStatsProcessor(
    ILogger<GameStatsProcessor> logger
) : ProcessorBase<Game>(logger)
{
    protected override Task OnProcessingAsync(Game entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not GameProcessingStatus.NeedsStatCalculation)
        {
            logger.LogTrace(
                "Game does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return Task.CompletedTask;
        }

        List<GameScore> verifiedScores =
        [
            .. entity.Scores
                .Where(s => s is
                    { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done })
                .OrderByDescending(s => s.Score)
        ];

        AssignScorePlacements(verifiedScores);
        entity.Rosters = GenerateRosters(verifiedScores);

        entity.ProcessingStatus = GameProcessingStatus.Done;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Assigns a <see cref="GameScore.Placement"/> for a given list of <see cref="GameScore"/>s
    /// </summary>
    /// <param name="scores">List of <see cref="GameScore"/>s</param>
    public static void AssignScorePlacements(IEnumerable<GameScore> scores)
    {
        foreach (var p in scores.OrderByDescending(s => s.Score).Select((s, idx) => new { Score = s, Index = idx + 1 }))
        {
            p.Score.Placement = p.Index;
        }
    }

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
}
