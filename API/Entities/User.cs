using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("users")]
public class User : EntityBase
{
	public int PlayerId { get; set; }
	public DateTime LastLogin { get; set; }
}