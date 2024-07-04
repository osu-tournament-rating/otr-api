namespace Database.Enums.Verification;

/// <summary>
/// The reason why a <see cref="Entities.Tournament"/> is rejected
/// </summary>
[Flags]
public enum TournamentRejectionReason
{
    /// <summary>
    /// The <see cref="Entities.Tournament"/> is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="Entities.Tournament"/> has no <see cref="Entities.Tournament.Matches"/> with a
    /// <see cref="VerificationStatus"/> of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/>
    /// </summary>
    NoVerifiedMatches = 1 << 0,

    /// <summary>
    /// The <see cref="Entities.Tournament"/>'s number of <see cref="Entities.Tournament.Matches"/> with a
    /// <see cref="VerificationStatus"/> of <see cref="VerificationStatus.Verified"/> or
    /// <see cref="VerificationStatus.PreVerified"/> is below 80% of the total
    /// </summary>
    NotEnoughVerifiedMatches = 1 << 1,

    /// <summary>
    /// The <see cref="Entities.Tournament"/>'s win condition is not <see cref="ScoringType.ScoreV2"/>
    /// </summary>
    /// <remarks>
    /// Only assigned via a "rejected submission". <br/>
    /// Covers cases such as gimmicky win conditions, mixed win conditions, etc
    /// </remarks>
    AbnormalWinCondition = 1 << 2,

    /// <summary>
    /// The <see cref="Entities.Tournament"/>'s format is not suitable for ratings
    /// </summary>
    /// <remarks>
    /// Only assigned via a "rejected submission". <br/>
    /// Covers cases such as excessive gimmicks, relax, multiple modes, etc
    /// </remarks>
    AbnormalFormat = 1 << 3,

    /// <summary>
    /// The <see cref="Entities.Tournament"/>'s team sizes are not consistent.
    /// </summary>
    /// <remarks>
    /// Only assigned via a "rejected submission". <br/>
    /// Covers cases such as &gt; 2 teams in lobby at once, async lobbies, team size gimmicks, varying team sizes, etc
    /// </remarks>
    VaryingTeamSize = 1 << 4,

    /// <summary>
    /// The <see cref="Entities.Tournament"/>'s data is incomplete or not recoverable
    /// Covers cases where match links are lost to time, private,
    /// main sheet is deleted, missing rounds, etc.
    /// </summary>
    /// <remarks>
    /// Only assigned via a "rejected submission". <br/>
    /// Covers cases where match links are lost to time / dead / private, main sheet is deleted, missing rounds, etc
    /// </remarks>
    IncompleteData = 1 << 5
}
