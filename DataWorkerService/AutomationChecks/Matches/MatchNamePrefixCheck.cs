using Database.Entities;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Database.Entities.Match"/>es that have inconsistent lobby names
/// </summary>
public class MatchNamePrefixCheck(ILogger<MatchNamePrefixCheck> logger) : AutomationCheckBase<Match>(logger)
{
    protected override bool OnChecking(Match entity) =>
        entity.Name.StartsWith(entity.Tournament.Abbreviation, StringComparison.OrdinalIgnoreCase);

    protected override void OnFail(Match entity)
    {
        entity.RejectionReason |= MatchRejectionReason.NamePrefixMismatch;
        base.OnFail(entity);
    }
}
