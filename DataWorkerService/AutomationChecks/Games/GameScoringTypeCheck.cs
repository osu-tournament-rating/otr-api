using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Games;

/// <summary>
/// Checks for <see cref="Game"/>s played with a <see cref="ScoringType"/>
/// that is not <see cref="ScoringType.ScoreV2"/>
/// </summary>
public class GameScoringTypeCheck(ILogger<GameScoringTypeCheck> logger) : AutomationCheckBase<Game>(logger)
{
    protected override bool OnChecking(Game entity) =>
        entity.ScoringType == ScoringType.ScoreV2;

    protected override void OnFail(Game entity)
    {
        entity.RejectionReason |= GameRejectionReason.InvalidScoringType;
        base.OnFail(entity);
    }
}
