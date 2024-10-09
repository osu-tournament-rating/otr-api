using Database.Enums.Verification;
using Match = Database.Entities.Match;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Database.Entities.Match"/>es that have inconsistent lobby names
/// </summary>
public class MatchNameCheck(ILogger<MatchNameCheck> logger) : AutomationCheckBase<Match>(logger)
{
    protected override bool OnChecking(Match entity) =>
        entity.Name.StartsWith(entity.Tournament.Abbreviation, StringComparison.OrdinalIgnoreCase);

    protected override void OnFail(Match entity)
    {
        entity.RejectionReason |= MatchRejectionReason.InvalidName;
        base.OnFail(entity);
    }
}
