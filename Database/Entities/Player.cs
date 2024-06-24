using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Processor;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Represents a player
/// </summary>
[Table("players")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Player : UpdateableEntityBase
{
    private string _username = string.Empty;
    private string _country = string.Empty;

    /// <summary>
    /// osu! id
    /// </summary>
    [Column("osu_id")]
    public long OsuId { get; set; }

    /// <summary>
    /// osu! username of the player
    /// </summary>
    [MaxLength(32)]
    [Column("username")]
    public string Username
    {
        get => string.IsNullOrEmpty(_username) ? $"Player {Id}" : _username;
        set => _username = value;
    }

    /// <summary>
    /// ISO country code
    /// </summary>
    [MaxLength(4)]
    [Column("country")]
    public string Country
    {
        get => string.IsNullOrEmpty(_country) ? "XX" : _country;
        set => _country = value;
    }

    /// <summary>
    /// <see cref="Enums.Ruleset"/> as set on the <see cref="Player"/>'s osu! profile
    /// </summary>
    [Column("default_ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Timestamp for the last time <see cref="RulesetData"/> was updated with data from the osu! API
    /// </summary>
    [Column("osu_last_fetch")]
    public DateTime OsuLastFetch { get; set; }

    /// <summary>
    /// Timestamp for the last time <see cref="RulesetData"/> was updated with data from the osu!Track API
    /// </summary>
    [Column("osu_track_last_fetch")]
    public DateTime OsuTrackLastFetch { get; set; }

    // Column name and value initialization is handled via OtrContext
    /// <summary>
    /// A collection of <see cref="PlayerRulesetData"/>, one for each <see cref="Enums.Ruleset"/>
    /// </summary>
    public ICollection<PlayerRulesetData> RulesetData { get; set; } = new List<PlayerRulesetData>();

    /// <summary>
    /// The <see cref="User"/> that owns the <see cref="Player"/>
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// A collection of <see cref="PlayerRating"/> owned by the <see cref="Player"/>
    /// </summary>
    public ICollection<PlayerRating> Ratings { get; set; } = new List<PlayerRating>();

    /// <summary>
    /// A collection of <see cref="RatingAdjustment"/>s adjusting any <see cref="Ratings"/>
    /// </summary>
    public ICollection<RatingAdjustment> RatingAdjustments { get; set; } = new List<RatingAdjustment>();

    /// <summary>
    /// A collection of <see cref="GameScore"/>s set by the <see cref="Player"/>
    /// </summary>
    public ICollection<GameScore> Scores { get; set; } = new List<GameScore>();

    /// <summary>
    /// A collection of <see cref="PlayerMatchStats"/> generated for the <see cref="Player"/>
    /// </summary>
    public IEnumerable<PlayerMatchStats> MatchStats { get; set; } = new List<PlayerMatchStats>();
}
