namespace API.Enums;

public enum MatchVerificationSource
{
	System,
	Admin,
	/// <summary>
	/// Someone who has the ability to verify matches
	/// </summary>
	MatchVerifier
}

/// <summary>
///  An enum representing the status of a match verification.
///  Verified matches are considered legitimate and will be used in the rating algorithm.
/// </summary>
public enum MatchVerificationStatus
{
	/// <summary>
	///  Fully verified as legitimate and will be incorporated into the rating algorithm.
	/// </summary>
	Verified = 0,
	/// <summary>
	///  A match that has been submitted for verification, but has not yet been processed by the system.
	/// </summary>
	PendingVerification = 1,
	/// <summary>
	///  Designates that the match title has been vetted by the system and it appears legitimate, but still needs to be
	///  reviewed.
	/// </summary>
	PreVerified = 2,
	/// <summary>
	///  Reviewed and determined to be illegitimate. Will not be used in the rating algorithm.
	/// </summary>
	Rejected = 3,
	/// <summary>
	///  Unable to verify and/or needs further review
	/// </summary>
	Unknown = 4,
	/// <summary>
	///  Automatic verification failed, requires human review
	/// </summary>
	Failure = 5
}

public enum GameVerificationStatus
{
	/// <summary>
	/// Game is confirmed accurate
	/// </summary>
	Verified = 0,
	/// <summary>
	/// Game passed all automation checks
	/// </summary>
	PreVerified = 1,
	/// <summary>
	/// Game was rejected for some reason (i.e. warmup, game mode mismatch, team size mismatch, etc.)
	/// </summary>
	Rejected = 2
}

public enum GameRejectionReason
{
	/// <summary>
	/// Game was rejected because it was a warmup according to automated checks
	/// </summary>
	FailedAutomationChecks = 0,
	/// <summary>
	/// Manually flagged as a warmup
	/// </summary>
	Warmup = 1,
	/// <summary>
	/// This match appears to be a non-competitive match (perhaps there was a cheater or something else)
	/// </summary>
	NotCompetitive = 2
}