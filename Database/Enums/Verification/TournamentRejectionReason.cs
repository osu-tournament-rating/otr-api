namespace Database.Enums.Verification;

/// <summary>
/// The reason why a tournament is rejected
/// </summary>
[Flags]
public enum TournamentRejectionReason
{
    /// <summary>
    /// The tournament is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The tournament has no 'Verified' or 'PreVerified' matches
    /// </summary>
    NoVerifiedMatches = 1 << 0,

    /// <summary>
    /// The tournament has &lt;80% of matches marked as 'Verified' or 'PreVerified'
    /// </summary>
    NotEnoughVerifiedMatches = 1 << 1,

    /// <summary>
    /// Tournament's win condition is not 'ScoreV2'.
    /// Covers cases such as gimmicky win conditions, mixed win conditions, etc.
    /// </summary>
    AbnormalWinCondition = 1 << 2,

    /// <summary>
    /// Tournament's format is not suitable for ratings.
    /// Covers cases such as excessive gimmicks, relax, multiple modes, etc.
    /// </summary>
    AbnormalFormat = 1 << 3,

    /// <summary>
    /// Tournament's team sizes are not consistent.
    /// Covers cases such as >2 teams in lobby, team size gimmicks,
    /// and varying team sizes.
    /// </summary>
    VaryingTeamSize = 1 << 4,

    /// <summary>
    /// Tournament's match data is incomplete or not recoverable.
    /// Covers cases where match links are lost to time, private,
    /// main sheet is deleted, missing rounds, etc.
    /// </summary>
    IncompleteData = 1 << 5
}
