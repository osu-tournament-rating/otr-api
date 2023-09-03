using API.Entities.Bases;
using Dapper;
using Newtonsoft.Json;

namespace API.Entities;

[Table("players")]
public class Player : UpdateableEntityBase
{
	[Column("osu_id")]
	public long OsuId { get; set; }
	[Column("username")]
	public string? Username { get; set; }
	[Column("rank_standard")]
	public int? RankStandard { get; set; }
	[Column("rank_taiko")]
	public int? RankTaiko { get; set; }
	[Column("rank_catch")]
	public int? RankCatch { get; set; }
	[Column("rank_mania")]
	public int? RankMania { get; set; }

	[NotMapped]
	public ICollection<Rating>? Ratings { get; set; } // A user can have multiple ratings, one for each mode
	[NotMapped]
	public User? WebInfo { get; set; }
	[NotMapped]
	public ICollection<Match> Matches { get; set; }
	[NotMapped]
	public ICollection<RatingHistory>? RatingHistories { get; set; }
}