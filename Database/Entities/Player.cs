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
    public string? Username { get; set; }

    /// <summary>
    /// ISO country code
    /// </summary>
    [MaxLength(4)]
    [Column("country")]
    public string? Country { get; set; }

    /// <summary>
    /// <see cref="Enums.Ruleset"/> as set on the <see cref="Player"/>'s osu! profile
    /// </summary>
    [Column("default_ruleset")]
    public Ruleset? Ruleset { get; set; }

    /// <summary>
    /// Last recorded <see cref="Ruleset.Standard"/> rank
    /// </summary>
    [Column("rank_standard")]
    public int? RankStandard { get; set; }

    /// <summary>
    /// Last recorded <see cref="Ruleset.Taiko"/> rank
    /// </summary>
    [Column("rank_taiko")]
    public int? RankTaiko { get; set; }

    /// <summary>
    /// Last recorded <see cref="Ruleset.Catch"/> rank
    /// </summary>
    [Column("rank_catch")]
    public int? RankCatch { get; set; }

    /// <summary>
    /// Last recorded <see cref="Ruleset.Mania"/> rank
    /// </summary>
    [Column("rank_mania")]
    public int? RankMania { get; set; }

    /// <summary>
    /// Earliest known <see cref="Ruleset.Standard"/> rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_osu_global_rank")]
    public int? EarliestOsuGlobalRank { get; set; }

    /// <summary>
    /// Earliest known <see cref="Ruleset.Taiko"/> rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_taiko_global_rank")]
    public int? EarliestTaikoGlobalRank { get; set; }

    /// <summary>
    /// Earliest known <see cref="Ruleset.Catch"/> rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_catch_global_rank")]
    public int? EarliestCatchGlobalRank { get; set; }

    /// <summary>
    /// Earliest known <see cref="Ruleset.Mania"/> rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_mania_global_rank")]
    public int? EarliestManiaGlobalRank { get; set; }

    /// <summary>
    /// Date for the earliest known standard rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_osu_global_rank_date")]
    public DateTime? EarliestOsuGlobalRankDate { get; set; }

    /// <summary>
    /// Date for the earliest known <see cref="Ruleset.Taiko"/> rank available for the player after they started
    /// playing tournaments
    /// </summary>
    [Column("earliest_taiko_global_rank_date")]
    public DateTime? EarliestTaikoGlobalRankDate { get; set; }

    /// <summary>
    /// Date for the earliest known <see cref="Ruleset.Catch"/> rank available for the player after they started
    /// playing tournaments
    /// </summary>
    [Column("earliest_catch_global_rank_date")]
    public DateTime? EarliestCatchGlobalRankDate { get; set; }

    /// <summary>
    /// Date for the earliest known <see cref="Ruleset.Mania"/> rank available for the player after they started
    /// playing tournaments
    /// </summary>
    [Column("earliest_mania_global_rank_date")]
    public DateTime? EarliestManiaGlobalRankDate { get; set; }

    /// <summary>
    /// The <see cref="User"/> that owns the <see cref="Player"/>
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// A collection of <see cref="BaseStats"/> owned by the <see cref="Player"/>
    /// </summary>
    public ICollection<BaseStats> Ratings { get; set; } = new List<BaseStats>();

    /// <summary>
    /// A collection of <see cref="RatingAdjustment"/>s owned by the <see cref="Player"/>
    /// </summary>
    public ICollection<RatingAdjustment> RatingAdjustments { get; set; } = new List<RatingAdjustment>();

    /// <summary>
    /// A collection of <see cref="MatchRatingAdjustment"/> owned by the <see cref="Player"/>
    /// </summary>
    public ICollection<MatchRatingAdjustment> MatchRatingAdjustments { get; set; } = new List<MatchRatingAdjustment>();

    /// <summary>
    /// A collection of <see cref="GameScore"/>s owned by the <see cref="Player"/>
    /// </summary>
    public ICollection<GameScore> Scores { get; set; } = new List<GameScore>();

    /// <summary>
    /// A collection of <see cref="PlayerMatchStats"/> owned by the <see cref="Player"/>
    /// </summary>
    public IEnumerable<PlayerMatchStats> MatchStats { get; set; } = new List<PlayerMatchStats>();
}
