using API.Entities.Bases;
using Dapper;

namespace API.Entities;

[Table("multiplayerlinks")]
public class MultiplayerLink : UpdateableEntityBase
{
	[Column("mp_link_id")]
	public long MpLinkId { get; set; }
	/// <summary>
	///  PENDING:  Waiting to be processed by worker
	///  REVIEW:   Processed by worker, needs human review before acception
	///  ACCEPTED: Marked as verified tournament match - all matches with this
	///  status should be converted to MatchData / are in the matchdata table already
	///  REJECTED: Will be ignored by MatchData converter
	/// </summary>
	[Column("status")]
	public string Status { get; set; } = "PENDING";
	[Column("lobby_name")]
	public string LobbyName { get; set; } = string.Empty;
}