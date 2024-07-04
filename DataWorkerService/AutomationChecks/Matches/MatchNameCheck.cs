using System.Text.RegularExpressions;
using Database.Enums.Verification;
using Match = Database.Entities.Match;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Database.Entities.Match"/>es that have inconsistent lobby names
/// </summary>
public class MatchNameCheck(ILogger<MatchNameCheck> logger) : AutomationCheckBase<Match>(logger)
{
    public static readonly List<string> Patterns = [@"^[^\n\r]*(\(.+\)\s*vs\.?\s*\(.+\)).*$"];

    protected override bool OnChecking(Match entity) =>
        entity.Name.StartsWith(entity.Tournament.Abbreviation, StringComparison.OrdinalIgnoreCase)
        && Patterns.Any(pattern => Regex.IsMatch(entity.Name, pattern, RegexOptions.IgnoreCase));

    protected override void OnFail(Match entity)
    {
        entity.RejectionReason |= MatchRejectionReason.InvalidName;
        base.OnFail(entity);
    }
}
