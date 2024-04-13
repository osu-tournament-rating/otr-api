using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Osu;
using Microsoft.EntityFrameworkCore;

namespace API.Entities;

[Table("players")]
[Index("OsuId", Name = "Players_osuid", IsUnique = true)]
public class Player
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("osu_id")]
    public long OsuId { get; set; }

    [Column("created", TypeName = "timestamp with time zone")]
    public DateTime Created { get; set; }

    [Column("rank_standard")]
    public int? RankStandard { get; set; }

    [Column("rank_taiko")]
    public int? RankTaiko { get; set; }

    [Column("rank_catch")]
    public int? RankCatch { get; set; }

    [Column("rank_mania")]
    public int? RankMania { get; set; }

    [Column("updated", TypeName = "timestamp with time zone")]
    public DateTime? Updated { get; set; }

    [Column("username")]
    [MaxLength(32)]
    public string? Username { get; set; }

    [Column("country")]
    [MaxLength(4)]
    public string? Country { get; set; }

    [Column("default_ruleset")]
    public OsuEnums.Mode? Ruleset { get; set; }

    [Column("earliest_osu_global_rank")]
    public int? EarliestOsuGlobalRank { get; set; }

    [Column("earliest_mania_global_rank")]
    public int? EarliestManiaGlobalRank { get; set; }

    [Column("earliest_taiko_global_rank")]
    public int? EarliestTaikoGlobalRank { get; set; }

    [Column("earliest_catch_global_rank")]
    public int? EarliestCatchGlobalRank { get; set; }

    [Column("earliest_osu_global_rank_date")]
    public DateTime? EarliestOsuGlobalRankDate { get; set; }

    [Column("earliest_mania_global_rank_date")]
    public DateTime? EarliestManiaGlobalRankDate { get; set; }

    [Column("earliest_taiko_global_rank_date")]
    public DateTime? EarliestTaikoGlobalRankDate { get; set; }

    [Column("earliest_catch_global_rank_date")]
    public DateTime? EarliestCatchGlobalRankDate { get; set; }

    [InverseProperty("Player")]
    public virtual IEnumerable<MatchRatingStats> MatchRatingStats { get; set; } = null!;

    [InverseProperty("Player")]
    public virtual ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();

    [InverseProperty("Player")]
    public virtual IEnumerable<PlayerMatchStats> MatchStats { get; set; } = null!;

    [InverseProperty("Player")]
    public virtual ICollection<RatingAdjustment> RatingAdjustments { get; set; } =
        new List<RatingAdjustment>();

    [InverseProperty("Player")]
    public virtual ICollection<BaseStats> Ratings { get; set; } = new List<BaseStats>();

    [InverseProperty("Player")]
    public virtual User? User { get; set; }
}
