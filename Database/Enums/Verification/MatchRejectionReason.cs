namespace Database.Enums.Verification;

/// <summary>
/// The reason why a <see cref="Entities.Match"/> is rejected
/// </summary>
[Flags]
public enum MatchRejectionReason
{
    /// <summary>
    /// The <see cref="Entities.Match"/> is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The osu! API returned invalid data or no data for the <see cref="Entities.Match"/>
    /// </summary>
    NoData = 1 << 0,

    /// <summary>
    /// The <see cref="Entities.Match"/>'s <see cref="Entities.Match.Name"/> does not follow tournament lobby title conventions
    /// </summary>
    InvalidName = 1 << 1,

    /// <summary>
    /// The <see cref="Entities.Match"/> has no <see cref="Entities.Match.Games"/> with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/>
    /// </summary>
    NoVerifiedGames = 1 << 2,

    /// <summary>
    /// The <see cref="Entities.Match"/>'s number of <see cref="Entities.Match.Games"/> with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/> is not an odd number
    /// (does not satisfy "best of X")
    /// </summary>
    UnexpectedGameCount = 1 << 3,

    /// <summary>
    /// The <see cref="Entities.Match"/>'s <see cref="Entities.Match.EndTime"/> could not be determined
    /// </summary>
    NoEndTime = 1 << 4
}
