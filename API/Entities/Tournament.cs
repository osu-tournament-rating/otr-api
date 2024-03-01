using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("tournaments")]
public class Tournament
{
	[Column("id")]
	public int Id { get; set; }
	[Column("name")]
	[MaxLength(512)]
	public string Name { get; set; } = null!;
	[Column("abbreviation")]
	[MaxLength(32)]
	public string Abbreviation { get; set; } = null!;
	[Column("forum_url")]
	[MaxLength(255)]
	public string ForumUrl { get; set; } = null!;
	[Column("rank_range_lower_bound")]
	public int RankRangeLowerBound { get; set; }
	[Column("mode")]
	public int Mode { get; set; }
	[Column("team_size")]
	public int TeamSize { get; set; }
	[Column("submitter_id")]
	public int? SubmitterUserId { get; set; }
	[Column("created", TypeName = "timestamp with time zone")]
	public DateTime Created { get; set; }
	[Column("updated", TypeName = "timestamp with time zone")]
	public DateTime? Updated { get; set; }
	[InverseProperty("Tournament")]
	public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
	public virtual User? SubmittedBy { get; set; }
}