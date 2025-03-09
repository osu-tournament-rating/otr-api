using Common.Enums.Enums;

namespace API.DTOs;

/// <summary>
/// Represents user controlled settings for otr-web
/// </summary>
public class UserSettingsDTO
{
    /// <summary>
    /// Preferred ruleset of the associated user
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Denotes whether the associated user has overwritten their default ruleset
    /// </summary>
    /// <remarks>
    /// If false, the default ruleset is always the same as the user's default ruleset on the osu! website
    /// </remarks>
    public bool RulesetIsControlled { get; init; }
}
