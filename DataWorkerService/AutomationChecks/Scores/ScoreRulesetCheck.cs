using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Scores;

/// <summary>
/// Checks for <see cref="GameScore"/>s set in a <see cref="Ruleset"/>
/// differing from the parent <see cref="Tournament"/>
/// </summary>
public class ScoreRulesetCheck(ILogger<ScoreRulesetCheck> logger) : AutomationCheckBase<GameScore>(logger)
{
    protected override bool OnChecking(GameScore entity) =>
        entity.Ruleset == entity.Game.Match.Tournament.Ruleset;

    protected override void OnFail(GameScore entity)
    {
        entity.RejectionReason |= ScoreRejectionReason.RulesetMismatch;
        base.OnFail(entity);
    }
}
