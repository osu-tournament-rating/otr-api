using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("players")]
public class Player : EntityBase
{
	[Column("osu_id")]
	public long OsuId { get; set; }

	[NotMapped]
	public ICollection<Rating>? Ratings { get; set; } // A user can have multiple ratings, one for each mode
	[NotMapped]
	public User? WebInfo { get; set; }
	[NotMapped]
	public ICollection<Match> Matches { get; set; }
	[NotMapped]
	public ICollection<RatingHistory>? RatingHistories { get; set; }
}