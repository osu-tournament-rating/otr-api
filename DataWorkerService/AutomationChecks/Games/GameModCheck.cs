using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Games;

/// <summary>
/// Checks for <see cref="Game"/>s played using any <see cref="Mods.InvalidMods"/>
/// </summary>
public class GameModCheck(ILogger<GameModCheck> logger) : AutomationCheckBase<Game>(logger)
{
    protected override bool OnChecking(Game entity) =>
        entity.Mods.HasFlag(Mods.InvalidMods);

    protected override void OnFail(Game entity)
    {
        entity.RejectionReason |= GameRejectionReason.InvalidMods;
        base.OnFail(entity);
    }
}
