using API.Entities.Bases;
using API.Osu;
using Dapper;

namespace API.Entities;

[Table("ratinghistories")]
public class RatingHistory : EntityBase
{
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("mu")]
	public double Mu { get; set; }
	[Column("sigma")]
	public double Sigma { get; set; }
	[Column("match_data_id")]
	public int MatchDataId { get; set; }
	[Column("mode")]
	public OsuEnums.Mode Mode { get; set; }
	[NotMapped]
	public long MatchId { get; set; }
	[NotMapped]
	public long GameId { get; set; }
}