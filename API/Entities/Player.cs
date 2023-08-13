using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("players")]
public class Player : EntityBase
{
	[Column("osu_id")]
	public long OsuId { get; set; }
}