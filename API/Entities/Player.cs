using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using API.Osu.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Entities;

/// <summary>
/// Represents a player
/// </summary>
[Table("players")]
[Index("OsuId", Name = "Players_osuid", IsUnique = true)]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Player
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// osu! id of the player
    /// </summary>
    [Column("osu_id")]
    public long OsuId { get; set; }

    /// <summary>
    /// Date the entity was created
    /// </summary>
    [Column("created", TypeName = "timestamp with time zone")]
    public DateTime Created { get; set; }

    /// <summary>
    /// Last recorded osu! standard rank for the player
    /// </summary>
    [Column("rank_standard")]
    public int? RankStandard { get; set; }

    /// <summary>
    /// Last recorded osu! taiko rank for the player
    /// </summary>
    [Column("rank_taiko")]
    public int? RankTaiko { get; set; }

    /// <summary>
    /// Last recorded osu! catch rank for the player
    /// </summary>

    [Column("rank_catch")]
    public int? RankCatch { get; set; }

    /// <summary>
    /// Last recorded osu! mania rank for the player
    /// </summary>
    [Column("rank_mania")]
    public int? RankMania { get; set; }

    /// <summary>
    /// Date of the last update to the entity
    /// </summary>
    [Column("updated", TypeName = "timestamp with time zone")]
    public DateTime? Updated { get; set; }

    /// <summary>
    /// osu! username of the player
    /// </summary>
    [Column("username")]
    [MaxLength(32)]
    public string? Username { get; set; }

    /// <summary>
    /// osu! country code of the player
    /// </summary>
    [Column("country")]
    [MaxLength(4)]
    public string? Country { get; set; }

    /// <summary>
    /// Preferred ruleset of the player
    /// </summary>
    [Column("default_ruleset")]
    public Ruleset? Ruleset { get; set; }

    /// <summary>
    /// Earliest known standard rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_osu_global_rank")]
    public int? EarliestOsuGlobalRank { get; set; }

    /// <summary>
    /// Earliest known mania rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_mania_global_rank")]
    public int? EarliestManiaGlobalRank { get; set; }

    /// <summary>
    /// Earliest known taiko rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_taiko_global_rank")]
    public int? EarliestTaikoGlobalRank { get; set; }

    /// <summary>
    /// Earliest known catch rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_catch_global_rank")]
    public int? EarliestCatchGlobalRank { get; set; }

    /// <summary>
    /// Date for the earliest known standard rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_osu_global_rank_date")]
    public DateTime? EarliestOsuGlobalRankDate { get; set; }

    /// <summary>
    /// Date for the earliest known mania rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_mania_global_rank_date")]
    public DateTime? EarliestManiaGlobalRankDate { get; set; }

    /// <summary>
    /// Date for the earliest known taiko rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_taiko_global_rank_date")]
    public DateTime? EarliestTaikoGlobalRankDate { get; set; }

    /// <summary>
    /// Date for the earliest known catch rank available for the player after they started playing tournaments
    /// </summary>
    [Column("earliest_catch_global_rank_date")]
    public DateTime? EarliestCatchGlobalRankDate { get; set; }

    /// <summary>
    /// All match rating stats for the player
    /// </summary>
    [InverseProperty("Player")]
    public virtual IEnumerable<MatchRatingStats> MatchRatingStats { get; set; } = null!;

    /// <summary>
    /// All match scores for the player
    /// </summary>
    [InverseProperty("Player")]
    public virtual ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();

    /// <summary>
    /// All match stats for the player
    /// </summary>
    [InverseProperty("Player")]
    public virtual IEnumerable<PlayerMatchStats> MatchStats { get; set; } = null!;

    /// <summary>
    /// All rating adjustments for the player
    /// </summary>
    [InverseProperty("Player")]
    public virtual ICollection<RatingAdjustment> RatingAdjustments { get; set; } =
        new List<RatingAdjustment>();

    /// <summary>
    /// All o!tr ratings for the player
    /// </summary>
    [InverseProperty("Player")]
    public virtual ICollection<BaseStats> Ratings { get; set; } = new List<BaseStats>();

    /// <summary>
    /// The associated user for the player
    /// </summary>
    [InverseProperty("Player")]
    public virtual User? User { get; set; }
}
