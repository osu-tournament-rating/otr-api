using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Games;

/// <summary>
/// Checks for <see cref="Game"/>s played with a <see cref="Database.Enums.TeamType"/>
/// that is not <see cref="Database.Enums.TeamType.TeamVs"/>
/// </summary>
public class GameTeamTypeCheck(ILogger<GameTeamTypeCheck> logger) : AutomationCheckBase<Game>(logger)
{
    protected override bool OnChecking(Game entity) =>
        entity.TeamType == TeamType.TeamVs;

    protected override void OnFail(Game entity)
    {
        entity.RejectionReason |= GameRejectionReason.InvalidTeamType;
        base.OnFail(entity);
    }
}
