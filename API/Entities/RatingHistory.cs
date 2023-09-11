using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("ratinghistories")]
[Index("PlayerId", "MatchId", Name = "ratinghistories_pk", IsUnique = true)]
public partial class RatingHistory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("player_id")]
    public int PlayerId { get; set; }

    [Column("mu")]
    public double Mu { get; set; }

    [Column("sigma")]
    public double Sigma { get; set; }

    [Column("created", TypeName = "timestamp with time zone")]
    public DateTime Created { get; set; }

    [Column("mode")]
    public int Mode { get; set; }

    [Column("match_id")]
    public int MatchId { get; set; }

    [Column("updated", TypeName = "timestamp with time zone")]
    public DateTime? Updated { get; set; }

    [ForeignKey("MatchId")]
    [InverseProperty("RatingHistories")]
    public virtual Match Match { get; set; } = null!;

    [ForeignKey("PlayerId")]
    [InverseProperty("RatingHistories")]
    public virtual Player Player { get; set; } = null!;
}
