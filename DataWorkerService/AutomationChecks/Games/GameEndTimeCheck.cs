using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.Utilities.Extensions;

namespace DataWorkerService.AutomationChecks.Games;

/// <summary>
/// Checks for <see cref="Game"/>s where the <see cref="Game.EndTime"/> could not be determined
/// </summary>
public class GameEndTimeCheck(ILogger<GameEndTimeCheck> logger) : AutomationCheckBase<Game>(logger)
{
    protected override bool OnChecking(Game entity) =>
        !entity.EndTime.IsPlaceholder();

    protected override void OnFail(Game entity)
    {
        entity.RejectionReason |= GameRejectionReason.NoEndTime;
        base.OnFail(entity);
    }
}
