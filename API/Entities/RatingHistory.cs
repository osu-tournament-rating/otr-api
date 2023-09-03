using API.Entities.Bases;
using API.Osu;
using Dapper;

namespace API.Entities;

[Table("ratinghistories")]
public class RatingHistory : UpdateableEntityBase
{
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("mu")]
	public double Mu { get; set; }
	[Column("sigma")]
	public double Sigma { get; set; }
	[Column("game_id")]
	public int GameId { get; set; }
	[Column("mode")]
	public OsuEnums.Mode Mode { get; set; }
}