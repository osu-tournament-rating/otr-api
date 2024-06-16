using System.ComponentModel.DataAnnotations.Schema;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// <see cref="Entities.User"/> controlled values that define behaviors on the o!TR website
/// </summary>
[Table("user_settings")]
public class UserSettings : UpdateableEntityBase
{
    /// <summary>
    /// Default <see cref="Ruleset"/> for browsing
    /// </summary>
    [Column("default_ruleset")]
    public Ruleset DefaultRuleset { get; set; }

    /// <summary>
    /// Denotes if the user has overwritten the <see cref="DefaultRuleset"/>
    /// </summary>
    /// <remarks>
    /// If false, <see cref="DefaultRuleset"/> will sync to the <see cref="Ruleset"/> selected on the user's osu! profile
    /// </remarks>
    [Column("default_ruleset_controlled")]
    public bool DefaultRulesetIsControlled { get; set; }

    /// <summary>
    /// Id of the associated <see cref="User"/>
    /// </summary>
    [Column("user_id")]
    public int UserId { get; init; }
}
