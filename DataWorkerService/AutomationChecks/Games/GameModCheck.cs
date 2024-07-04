using Database.Entities;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Games;

/// <summary>
/// Checks for <see cref="Game"/>s played using any <see cref="Constants.InvalidMods"/>
/// </summary>
public class GameModCheck(ILogger<GameModCheck> logger) : AutomationCheckBase<Game>(logger)
{
    protected override bool OnChecking(Game entity) =>
        Constants.InvalidMods.All(m => !entity.Mods.HasFlag(m));

    protected override void OnFail(Game entity)
    {
        entity.RejectionReason |= GameRejectionReason.InvalidMods;
        base.OnFail(entity);
    }
}
