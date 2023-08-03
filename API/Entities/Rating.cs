using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("ratings")]
public class Rating : UpdateableEntityBase
{
	public int PlayerId { get; set; }
	public double Mu { get; set; }
	public double Sigma { get; set; }
	public double MuInitial { get; set; }
	public double SigmaInitial { get; set; }
}