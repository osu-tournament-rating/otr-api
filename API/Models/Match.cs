using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Table("matches")]
[Index("MatchId", Name = "osumatches_matchid", IsUnique = true)]
public partial class Match
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("match_id")]
    public long MatchId { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("start_time", TypeName = "timestamp without time zone")]
    public DateTime? StartTime { get; set; }

    [Column("created", TypeName = "timestamp without time zone")]
    public DateTime Created { get; set; }

    [Column("updated", TypeName = "timestamp without time zone")]
    public DateTime? Updated { get; set; }

    [Column("end_time", TypeName = "timestamp without time zone")]
    public DateTime? EndTime { get; set; }

    [Column("verification_info")]
    public string? VerificationInfo { get; set; }

    [Column("verification_source")]
    public int? VerificationSource { get; set; }

    [Column("verification_status")]
    public int? VerificationStatus { get; set; }

    [InverseProperty("Match")]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    [InverseProperty("Game")]
    public virtual ICollection<RatingHistory> Ratinghistories { get; set; } = new List<RatingHistory>();
}
