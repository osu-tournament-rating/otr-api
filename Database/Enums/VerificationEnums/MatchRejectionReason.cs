namespace Database.Enums.VerificationEnums;

/// <summary>
/// The reason why a match is rejected
/// </summary>
[Flags]
public enum MatchRejectionReason
{
    /// <summary>
    /// Match is not rejected
    /// </summary>
    None = 0,
    /// <summary>
    /// osu! API returned invalid or no data for the match
    /// </summary>
    NoData = 1 << 0,
    /// <summary>
    /// Match name does not follow tournament lobby title conventions
    /// </summary>
    InvalidName = 1 << 1,
    /// <summary>
    /// Match does not contain any <see cref="Entities.Game"/>s marked "PreVerified" or "Verified"
    /// </summary>
    NoVerifiedGames = 1 << 2,
    /// <summary>
    /// Match does not contain an odd number of verified <see cref="Entities.Game"/>s
    /// (e.g. does not satisfy "best of X")
    /// </summary>
    UnexpectedGameCount = 1 << 3
}
