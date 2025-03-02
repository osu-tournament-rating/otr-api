using Database.Enums;

namespace Database.Entities;

/// <summary>
/// <see cref="Entities.User"/> controlled values that define behaviors on the o!TR website
/// </summary>
public class UserSettings : UpdateableEntityBase
{
    /// <summary>
    /// Default <see cref="Ruleset"/> for browsing
    /// </summary>
    public Ruleset DefaultRuleset { get; set; }

    /// <summary>
    /// Denotes if the user has overwritten the <see cref="DefaultRuleset"/>
    /// </summary>
    /// <remarks>
    /// If false, <see cref="DefaultRuleset"/> will sync to the <see cref="Ruleset"/> selected on the user's osu! profile
    /// </remarks>
    public bool DefaultRulesetIsControlled { get; set; }

    /// <summary>
    /// Id of the associated <see cref="User"/>
    /// </summary>
    public int UserId { get; init; }
}
