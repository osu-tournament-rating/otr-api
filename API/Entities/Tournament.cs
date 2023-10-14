using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("tournaments")]
public class Tournament
{
	[Column("id")]
	public int Id { get; set; }
	[Column("name")]
	public string Name { get; set; } = null!;
	[Column("abbreviation")]
	public string Abbreviation { get; set; } = null!;
	[Column("forum_url")]
	public string ForumUrl { get; set; } = null!;
	[Column("rank_range_lower_bound")]
	public int RankRangeLowerBound { get; set; }
	[Column("mode")]
	public int Mode { get; set; }
	[Column("team_size")]
	public int TeamSize { get; set; }
	[Column("created", TypeName = "timestamp with time zone")]
	public DateTime Created { get; set; }
	[Column("updated", TypeName = "timestamp with time zone")]
	public DateTime? Updated { get; set; }
	[InverseProperty("Tournament")]
	public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}