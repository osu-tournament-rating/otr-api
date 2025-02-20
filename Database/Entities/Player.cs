using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using Database.Entities.Processor;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Represents a player
/// </summary>
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Player : UpdateableEntityBase, IAdminNotableEntity<PlayerAdminNote>
{
    private string _username = string.Empty;
    private string _country = string.Empty;

    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// osu! username
    /// </summary>
    [MaxLength(32)]
    public string Username
    {
        get => string.IsNullOrEmpty(_username) ? $"Player {Id}" : _username;
        set => _username = value;
    }

    /// <summary>
    /// ISO country code
    /// </summary>
    [MaxLength(4)]
    public string Country
    {
        get => string.IsNullOrEmpty(_country) ? "XX" : _country;
        set => _country = value;
    }

    /// <summary>
    /// <see cref="Enums.Ruleset"/> as set on the <see cref="Player"/>'s osu! profile
    /// </summary>
    public Ruleset DefaultRuleset { get; set; }

    /// <summary>
    /// Timestamp for the last time <see cref="RulesetData"/> was updated with data from the osu! API
    /// </summary>
    public DateTime OsuLastFetch { get; set; }

    /// <summary>
    /// Timestamp for the last time <see cref="RulesetData"/> was updated with data from the osu!Track API
    /// </summary>
    public DateTime OsuTrackLastFetch { get; set; }

    /// <summary>
    /// A collection of <see cref="PlayerOsuRulesetData"/>, one for each <see cref="Enums.Ruleset"/>
    /// </summary>
    public ICollection<PlayerOsuRulesetData> RulesetData { get; set; } = [];

    /// <summary>
    /// The <see cref="User"/> that owns the <see cref="Player"/>
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// A collection of <see cref="PlayerRating"/> owned by the <see cref="Player"/>
    /// </summary>
    public ICollection<PlayerRating> Ratings { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="RatingAdjustment"/>s adjusting any <see cref="Ratings"/>
    /// </summary>
    public ICollection<RatingAdjustment> RatingAdjustments { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="GameScore"/>s set by the <see cref="Player"/>
    /// </summary>
    public ICollection<GameScore> Scores { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="PlayerMatchStats"/> generated for the <see cref="Player"/>
    /// </summary>
    public ICollection<PlayerMatchStats> MatchStats { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="PlayerTournamentStats"/> generated for the <see cref="Player"/>
    /// </summary>
    public ICollection<PlayerTournamentStats> TournamentStats { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="PlayerHighestRanks"/> for the <see cref="Player"/>
    /// representing the peak global & country ranks for each ruleset
    /// </summary>
    public ICollection<PlayerHighestRanks> HighestRanks { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="PlayerAdminNote"/> associated with the <see cref="Player"/>
    /// </summary>
    public ICollection<PlayerAdminNote> AdminNotes { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="Beatmapset"/> created by the <see cref="Player"/>
    /// </summary>
    public ICollection<Beatmapset> CreatedBeatmapsets { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="Beatmap"/> created by the <see cref="Player"/>
    /// </summary>
    public ICollection<Beatmap> CreatedBeatmaps { get; set; } = [];
}
