using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Scores;

/// <summary>
/// Checks for <see cref="GameScore"/>s with score below the <see cref="Constants.ScoreMinimum"/>
/// </summary>
public class ScoreMinimumCheck(ILogger<ScoreMinimumCheck> logger) : AutomationCheckBase<GameScore>(logger)
{
    protected override bool OnChecking(GameScore entity) =>
        entity.Score > Constants.ScoreMinimum;

    protected override void OnFail(GameScore entity)
    {
        entity.RejectionReason |= ScoreRejectionReason.ScoreBelowMinimum;
        base.OnFail(entity);
    }
}
