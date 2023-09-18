using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("players")]
[Index("OsuId", Name = "Players_osuid", IsUnique = true)]
public partial class Player
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
    public string? Username { get; set; }
    
    [Column("country")]
    public string? Country { get; set; }
    
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
    public virtual ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();

    [InverseProperty("Player")]
    public virtual ICollection<RatingHistory> RatingHistories { get; set; } = new List<RatingHistory>();

    [InverseProperty("Player")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [InverseProperty("Player")]
    public virtual User? User { get; set; }
    
}
