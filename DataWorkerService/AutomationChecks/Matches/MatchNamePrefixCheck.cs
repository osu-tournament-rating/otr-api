using Common.Enums.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Database.Entities.Match"/> <see cref="Database.Entities.Match.Name"/>s that
/// do not begin with the parent <see cref="Database.Entities.Tournament"/>'s
/// <see cref="Database.Entities.Tournament.Abbreviation"/>
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
