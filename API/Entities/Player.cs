using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("players")]
public class Player : EntityBase
{
	public long OsuId { get; set; }
}