using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Games;

/// <summary>
/// Checks for <see cref="Game"/>s played in a <see cref="Ruleset"/>
/// differing from the parent <see cref="Tournament"/>
/// </summary>
public class GameRulesetCheck(ILogger<GameRulesetCheck> logger) : AutomationCheckBase<Game>(logger)
{
    protected override bool OnChecking(Game entity) =>
        entity.Ruleset == entity.Match.Tournament.Ruleset;

    protected override void OnFail(Game entity)
    {
        entity.RejectionReason |= GameRejectionReason.RulesetMismatch;
        base.OnFail(entity);
    }
}
