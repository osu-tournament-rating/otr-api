using System.Text.RegularExpressions;
using Database.Enums.Verification;
using Match = Database.Entities.Match;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Database.Entities.Match"/>es that have inconsistent lobby names
/// </summary>
public class MatchNameFormatCheck(ILogger<MatchNameFormatCheck> logger) : AutomationCheckBase<Match>(logger)
{
    protected override bool OnChecking(Match entity) =>
        Constants.MatchNamePatterns.Any(pattern => Regex.IsMatch(entity.Name, pattern, RegexOptions.IgnoreCase));

    protected override void OnFail(Match entity)
    {
        entity.WarningFlags |= MatchWarningFlags.UnexpectedNameFormat;
        base.OnFail(entity);
    }
}
