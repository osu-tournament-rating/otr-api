namespace Database.Enums;

public enum Old_MatchVerificationSource
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
public enum Old_MatchVerificationStatus
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

public enum Old_GameVerificationStatus
{
    /// <summary>
    /// Game is confirmed accurate
    /// </summary>
    Verified = 0,

    /// <summary>
    /// Game passed all automation checks, still needs human verification
    /// </summary>
    PreVerified = 1,

    /// <summary>
    /// Game was rejected for some reason (i.e. warmup, game mode mismatch, team size mismatch, etc.)
    /// </summary>
    Rejected = 2
}

/// <summary>
/// Denotes why a game failed automation checks or is rejected
/// </summary>
[Flags]
public enum Old_GameRejectionReason
{
    /// <summary>
    /// There is an uneven number of players in the lobby (e.g. 2v3),
    /// or a 1v1 is expected but it's a 1v2, etc.
    /// </summary>
    TeamSizeMismatch = 1 << 0,
    /// <summary>
    /// The ruleset of this game is not what is expected
    /// </summary>
    InvalidRuleset = 1 << 1,
    /// <summary>
    /// The game was played in something other than ScoreV2
    /// </summary>
    InvalidScoringType = 1 << 2,
    /// <summary>
    /// The game features invalid mods, or features scores with invalid mods
    /// </summary>
    InvalidMods = 1 << 3,
    /// <summary>
    /// The game has a different team type than expected.
    /// </summary>
    InvalidTeamType = 1 << 4,
    /// <summary>
    /// The game has no scores
    /// </summary>
    NoScores = 1 << 5
}
