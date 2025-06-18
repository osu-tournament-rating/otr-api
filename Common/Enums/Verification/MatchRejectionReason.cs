using System.Text.RegularExpressions;

namespace Common.Enums.Verification;

/// <summary>
/// The reason why a <see cref="Match"/> is rejected
/// </summary>
[Flags]
public enum MatchRejectionReason
{
    /// <summary>
    /// The <see cref="Match"/> is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The osu! API returned invalid data or no data for the <see cref="Match"/>
    /// </summary>
    NoData = 1 << 0,

    /// <summary>
    /// The osu! API returned no <see cref="Database.Entities.Game"/>s for the <see cref="Match"/>
    /// </summary>
    NoGames = 1 << 1,

    /// <summary>
    /// The <see cref="Match"/>'s <see cref="Match.Name"/> does not start with the
    /// parent <see cref="Database.Entities.Tournament"/>'s <see cref="Database.Entities.Tournament.Abbreviation"/>
    /// </summary>
    NamePrefixMismatch = 1 << 2,

    /// <summary>
    /// The <see cref="Match"/>'s <see cref="Entities.Games"/> were eligible for <see cref="TeamType.TeamVs"/>
    /// conversion and attempting <see cref="TeamType.TeamVs"/> conversion was not successful
    /// </summary>
    FailedTeamVsConversion = 1 << 3,

    /// <summary>
    /// The <see cref="Match"/> has no <see cref="Match.Games"/> with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/>
    /// </summary>
    NoValidGames = 1 << 4,

    /// <summary>
    /// The <see cref="Match"/>'s number of <see cref="Match.Games"/> with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/> is not an odd number
    /// (does not satisfy "best of X")
    /// </summary>
    UnexpectedGameCount = 1 << 5,

    /// <summary>
    /// The <see cref="Match"/>'s <see cref="Match.EndTime"/> could not be determined
    /// </summary>
    NoEndTime = 1 << 6,

    /// <summary>
    /// The <see cref="Database.Entities.Tournament"/> the <see cref="Match"/> was played in was rejected
    /// </summary>
    RejectedTournament = 1 << 7
}
