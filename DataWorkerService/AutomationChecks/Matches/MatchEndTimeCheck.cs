using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.Utilities.Extensions;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Match"/>es where the <see cref="Match.EndTime"/> could not be determined
/// </summary>
public class MatchEndTimeCheck(ILogger<MatchEndTimeCheck> logger) : AutomationCheckBase<Match>(logger)
{
    protected override bool OnChecking(Match entity) =>
        entity.EndTime is not null;

    protected override void OnFail(Match entity)
    {
        entity.RejectionReason |= MatchRejectionReason.NoEndTime;
        base.OnFail(entity);
    }
}
