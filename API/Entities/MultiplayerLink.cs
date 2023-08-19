using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("multiplayerlinks")]
public class MultiplayerLink : UpdateableEntityBase
{
	[Column("mp_link_id")]
	public long MpLinkId { get; set; }
	/// <summary>
	/// PENDING, REVIEW, ACCEPTED, REJECTED
	/// </summary>
	[Column("status")]
	public string Status { get; set; } = "PENDING";
}