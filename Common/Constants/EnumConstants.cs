using Common.Enums;

namespace Common.Constants;

public static class EnumConstants
{
    /// <summary>
    /// Rulesets which can be used to query external APIs for ruleset-specific data
    /// </summary>
    public static readonly Ruleset[] FetchableRulesets = [Ruleset.Osu, Ruleset.Taiko, Ruleset.Catch, Ruleset.ManiaOther];
}
