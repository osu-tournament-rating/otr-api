using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("base_stats")]
public class BaseStats
{
	[Key]
	[Column("id")]
	public int Id { get; set; }
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("mode")]
	public int Mode { get; set; }
	[Column("rating")]
	public double Rating { get; set; }
	[Column("volatility")]
	public double Volatility { get; set; }
	[Column("percentile")]
	public double Percentile { get; set; }
	[Column("global_rank")]
	public int GlobalRank { get; set; }
	[Column("country_rank")]
	public int CountryRank { get; set; }
	[Column("created", TypeName = "timestamp with time zone")]
	public DateTime Created { get; set; }
	[Column("updated", TypeName = "timestamp with time zone")]
	public DateTime? Updated { get; set; }
	[ForeignKey("PlayerId")]
	[InverseProperty("Ratings")]
	public virtual Player Player { get; set; } = null!;
}