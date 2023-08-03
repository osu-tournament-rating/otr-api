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
	[Column("mu_initial")]
	public double MuInitial { get; set; }
	[Column("sigma_initial")]
	public double SigmaInitial { get; set; }
}