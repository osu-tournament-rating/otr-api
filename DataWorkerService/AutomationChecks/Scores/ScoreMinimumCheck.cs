using Database.Entities;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Scores;

/// <summary>
/// Checks for <see cref="GameScore"/>s with score below the <see cref="MinimumScore"/>
/// </summary>
public class ScoreMinimumCheck(ILogger<ScoreMinimumCheck> logger) : AutomationCheckBase<GameScore>(logger)
{
    public const int MinimumScore = 1_000;

    protected override bool OnChecking(GameScore entity) =>
        entity.Score > MinimumScore;

    protected override void OnFail(GameScore entity)
    {
        entity.RejectionReason |= ScoreRejectionReason.ScoreBelowMinimum;
        base.OnFail(entity);
    }
}
