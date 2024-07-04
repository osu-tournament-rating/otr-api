using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Scores;

/// <summary>
/// Checks for <see cref="GameScore"/>s set using any <see cref="Mods.InvalidMods"/>
/// </summary>
public class ScoreModCheck(ILogger<ScoreModCheck> logger) : AutomationCheckBase<GameScore>(logger)
{
    protected override bool OnChecking(GameScore entity) =>
        entity.Mods.HasFlag(Mods.InvalidMods);

    protected override void OnFail(GameScore entity)
    {
        entity.RejectionReason |= ScoreRejectionReason.InvalidMods;
        base.OnFail(entity);
    }
}
