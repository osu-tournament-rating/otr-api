using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("ratinghistories")]
public class RatingHistory : EntityBase
{
	public int PlayerId { get; set; }
	public double Mu { get; set; }
	public double Sigma { get; set; }
}