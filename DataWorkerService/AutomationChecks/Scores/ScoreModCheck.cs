using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Scores;

/// <summary>
/// Checks for <see cref="GameScore"/>s set using any <see cref="Constants.InvalidMods"/>
/// </summary>
public class ScoreModCheck(ILogger<ScoreModCheck> logger) : AutomationCheckBase<GameScore>(logger)
{
    protected override bool OnChecking(GameScore entity) =>
        Constants.InvalidMods.All(m => !entity.Mods.HasFlag(m));

    protected override void OnFail(GameScore entity)
    {
        entity.RejectionReason |= ScoreRejectionReason.InvalidMods;
        base.OnFail(entity);
    }
}
