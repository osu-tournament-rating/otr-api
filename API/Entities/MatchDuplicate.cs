using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
///  Any matches that are duplicates are stored here. We track the osu! match id and link it to
///  a "root" match in the Matches table.
///  Take the case of an osu! match that is split up across 3 different lobbies
///  (probably due to bancho reasons, lobby breaking, etc.). This match would have one or both of:
///  1. The same osu! match id
///  2. The same match title
///  These would normally occupy 3 separate entries in the Matches table, but
///  we don't want to give rating points 3 separate times for the same match. So,
///  we store the osu match links here and delete the other entries. The two duplicates
///  will be tied to one match id (id => o!TR match id).
/// </summary>
[Table("match_duplicates")]
public class MatchDuplicate
{
	[Column("id")]
	public int Id { get; set; }
	/// <summary>
	///  The matchid of the duplicate
	/// </summary>
	[Column("matchId")]
	public int? MatchId { get; set; }
	/// <summary>
	///  The matchid we may believe this match is a duplicate of
	/// </summary>
	[Column("suspected_duplicate_of")]
	public int SuspectedDuplicateOf { get; set; }
	/// <summary>
	///  osu!'s match id of the duplicate lobby. This is here
	///  for preservation of data. Once we delete the duplicate match
	///  after merging, this will be the only record
	///  that maintains the osu! match id.
	/// </summary>
	[Column("osu_match_id")]
	public long OsuMatchId { get; set; }
	/// <summary>
	///  Has this item been reviewed by a human?
	///  true = Yes & confirmed to be a duplicate
	///  false = Yes & confirmed to not be a duplicate
	///  null = Not yet reviewed
	/// </summary>
	[Column("verified_duplicate")]
	public bool? VerifiedAsDuplicate { get; set; }
	/// <summary>
	///  The userid, if any, of the user who confirmed or denied this
	///  item as a duplicate of the root
	/// </summary>
	[Column("verified_by")]
	public int? VerifiedBy { get; set; }
	[InverseProperty("VerifiedDuplicates")]
	public User? Verifier { get; set; }
}