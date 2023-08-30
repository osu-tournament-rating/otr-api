using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("users")]
public class User : EntityBase
{
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("last_login")]
	public DateTime LastLogin { get; set; }
	
	// TODO: Add information about user roles (Admin, etc.) -- These could also be displayed in the frontend as a badge
}