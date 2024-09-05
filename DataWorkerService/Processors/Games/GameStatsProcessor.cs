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
            logger.LogDebug(
                "Game does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return Task.CompletedTask;
        }

        IEnumerable<GameScore> verifiedScores = entity.Scores
            .Where(s => s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done })
            .OrderByDescending(s => s.Score)
            .ToList();

        AssignScorePlacements(verifiedScores);
        entity.WinRecord = GenerateWinRecord(verifiedScores);

        entity.ProcessingStatus = GameProcessingStatus.Done;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Assigns a <see cref="GameScore.Placement"/> for a given list of <see cref="GameScore"/>s
    /// </summary>
    /// <param name="scores">List of <see cref="GameScore"/>s</param>
    public static void AssignScorePlacements(IEnumerable<GameScore> scores)
    {
        foreach (var p in scores.Select((s, idx) => new { Score = s, Index = idx + 1 }))
        {
            p.Score.Placement = p.Index;
        }
    }

    /// <summary>
    /// Generates a <see cref="GameWinRecord"/> for a given list of <see cref="GameScore"/>s
    /// </summary>
    /// <param name="scores">List of <see cref="GameScore"/>s</param>
    public static GameWinRecord GenerateWinRecord(IEnumerable<GameScore> scores)
    {
        var eScores = scores.ToList();

        Team winningTeam = eScores
            .GroupBy(s => s.Team)
            .Select(g => new { Team = g.Key, TotalScore = g.Sum(s => s.Score) })
            .OrderByDescending(t => t.TotalScore)
            .Select(t => t.Team)
            .First();

        Team losingTeam = winningTeam.OppositeTeam();

        return new GameWinRecord
        {
            WinnerTeam = winningTeam,
            LoserTeam = losingTeam,
            WinnerRoster = eScores.Where(s => s.Team == winningTeam).Select(s => s.PlayerId).ToArray(),
            LoserRoster = eScores.Where(s => s.Team == losingTeam).Select(s => s.PlayerId).ToArray(),
            WinnerScore = eScores.Where(s => s.Team == winningTeam).Sum(s => s.Score),
            LoserScore = eScores.Where(s => s.Team == losingTeam).Sum(s => s.Score)
        };
    }
}
