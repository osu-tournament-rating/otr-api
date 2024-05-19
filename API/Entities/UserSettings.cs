using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Interfaces;
using API.Osu.Enums;

namespace API.Entities;

/// <summary>
/// Represents a user's settings for otr-web
/// </summary>
[Table("user_settings")]
public class UserSettings : IUpdateableEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("created")]
    public DateTime Created { get; set; }

    [Column("updated")]
    public DateTime? Updated { get; set; }

    /// <summary>
    /// The id of the associated user
    /// </summary>
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// The default ruleset for the associated user
    /// </summary>
    [Column("default_ruleset")]
    public Ruleset? DefaultRuleset { get; set; }

    /// <summary>
    /// Denotes whether the associated user has overwritten their default ruleset
    /// </summary>
    /// <remarks>
    /// If false, the default ruleset is always the same as the user's default ruleset on the osu! website
    /// </remarks>
    [Column("default_ruleset_controlled")]
    public bool DefaultRulesetIsControlled { get; set; }

    /// <summary>
    /// The associated user
    /// </summary>
    [InverseProperty("Settings")]
    public User User { get; set; } = null!;
}
