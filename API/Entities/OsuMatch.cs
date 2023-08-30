using API.Entities.Bases;
using Dapper;

namespace API.Entities;

public enum MatchVerificationSource
{
	System,
	Admin
}

/// <summary>
/// An enum representing the status of a match verification.
/// Verified matches are considered legitimate and will be used in the rating algorithm.
/// </summary>
public enum VerificationStatus
{
	/// <summary>
	/// Fully verified as legitimate and will be incorporated into the rating algorithm.
	/// </summary>
	Verified = 0,
	/// <summary>
	/// A match that has been submitted for verification, but has not yet been processed by the system.
	/// </summary>
	PendingVerification = 1,
	/// <summary>
	/// Designates that the match title has been vetted by the system and it appears legitimate, but still needs to be reviewed.
	/// </summary>
	PreVerified = 2,		 
	/// <summary>
	/// Reviewed and determined to be illegitimate. Will not be used in the rating algorithm.
	/// </summary>
	Rejected = 3,		     
	/// <summary>
	/// Unable to verify and/or needs further review
	/// </summary>
	Unknown = 4,
	/// <summary>
	/// Verification failed, requires human review
	/// </summary>
	Failure = 5,
}

/// <summary>
/// Entity representing an osu! multiplayer match. Contains properties for
/// whether the match has been verified and rejected
/// </summary>
[Table("osumatches")]
public class OsuMatch : UpdateableEntityBase
{
	[Column("match_id")]
	public long MatchId { get; set; }
	
	[Column("name")]
	public string Name { get; set; } = null!;
	
	[Column("verification_source")]
	public MatchVerificationSource? VerificationSource { get; set; }
	
	[Column("verification_status")]
	public VerificationStatus? VerificationStatus { get; set; }
	
	[Column("verification_info")]
	public string? VerificationInfo { get; set; }
	
	[Column("start_time")]
	public DateTime StartTime { get; set; }
	
	[Column("end_time")]
	public DateTime? EndTime { get; set; }
}