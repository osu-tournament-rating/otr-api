using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("ratings")]
public class Rating : UpdateableEntityBase
{
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("mu")]
	public double Mu { get; set; }
	[Column("sigma")]
	public double Sigma { get; set; }
	[Column("mode")]
	public string Mode { get; set; } = "Standard"; // TODO: Remove hardcoded mode

	public Player Player { get; set; } = null!;
}