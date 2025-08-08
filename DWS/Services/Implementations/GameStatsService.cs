using Common.Enums.Verification;
using Database.Entities;
using DWS.Services.Interfaces;
using DWS.Utilities;

namespace DWS.Services.Implementations;

/// <summary>
/// Service for processing game statistics.
/// </summary>
public class GameStatsService(ILogger<GameStatsService> logger) : IGameStatsService
{
    /// <inheritdoc />
    public Task<bool> ProcessGameStatsAsync(Game entity)
    {
        if (entity.VerificationStatus is not VerificationStatus.Verified)
        {
            logger.LogError(
                "Stats processing was triggered for an unverified game, skipping [Id: {Id} | Verification Status: {Status}]",
                entity.Id,
                entity.VerificationStatus
            );

            return Task.FromResult(false);
        }

        List<GameScore> verifiedScores =
        [
            .. entity.Scores
                .Where(s => s.VerificationStatus == VerificationStatus.Verified)
                .OrderByDescending(s => s.Score)
        ];

        AssignScorePlacements(verifiedScores);

        // Clear existing rosters and regenerate to ensure consistency
        // This prevents duplicate key violations while ensuring data is up-to-date
        entity.Rosters.Clear();
        entity.Rosters = RostersHelper.GenerateRosters(verifiedScores);

        entity.LastProcessingDate = DateTime.UtcNow;

        return Task.FromResult(true);
    }

    /// <summary>
    /// Assigns a <see cref="GameScore.Placement"/> for a given list of <see cref="GameScore"/>s
    /// </summary>
    /// <param name="scores">List of <see cref="GameScore"/>s</param>
    private static void AssignScorePlacements(IEnumerable<GameScore> scores)
    {
        foreach (var p in scores.OrderByDescending(s => s.Score).Select((s, idx) => new { Score = s, Index = idx + 1 }))
        {
            p.Score.Placement = p.Index;
        }
    }
}
