using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("players")]
public class Player : EntityBase
{
	[Column("osu_id")]
	public long OsuId { get; set; }

	public Rating? Rating { get; set; }
	public ICollection<Match>? Matches { get; set; }
	public User? User { get; set; }
	public ICollection<RatingHistory>? RatingHistories { get; set; }
}