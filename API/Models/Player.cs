using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Table("players")]
[Index("OsuId", Name = "Players_osuid", IsUnique = true)]
public partial class Player
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("osu_id")]
    public long OsuId { get; set; }

    [Column("created", TypeName = "timestamp without time zone")]
    public DateTime Created { get; set; }

    [Column("rank_standard")]
    public int? RankStandard { get; set; }

    [Column("rank_taiko")]
    public int? RankTaiko { get; set; }

    [Column("rank_catch")]
    public int? RankCatch { get; set; }

    [Column("rank_mania")]
    public int? RankMania { get; set; }

    [Column("updated", TypeName = "timestamp without time zone")]
    public DateTime? Updated { get; set; }

    [Column("username")]
    public string? Username { get; set; }

    [InverseProperty("Player")]
    public virtual ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();

    [InverseProperty("Player")]
    public virtual ICollection<RatingHistory> RatingHistories { get; set; } = new List<RatingHistory>();

    [InverseProperty("Player")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [InverseProperty("Player")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
