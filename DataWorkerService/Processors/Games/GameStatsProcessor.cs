using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.Utilities;

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
        entity.Rosters = RostersHelper.GenerateRosters(verifiedScores);

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
}
