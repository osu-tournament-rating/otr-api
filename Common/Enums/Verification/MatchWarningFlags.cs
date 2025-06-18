using System.Text.RegularExpressions;

namespace Common.Enums.Verification;

/// <summary>
/// Warnings for irregularities in <see cref="Match"/> data that don't warrant an automatic
/// <see cref="VerificationStatus"/> of <see cref="VerificationStatus.PreRejected"/>
/// but should have attention drawn to them during manual review
/// </summary>
[Flags]
public enum MatchWarningFlags
{
    /// <summary>
    /// The <see cref="Match"/> has no warnings
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="Match"/>'s <see cref="Match.Name"/> does not follow common tournament
    /// lobby title conventions
    /// </summary>
    UnexpectedNameFormat = 1 << 0,

    /// <summary>
    /// The <see cref="Match"/>'s number of <see cref="Match.Games"/> is exactly 3 or 4
    /// </summary>
    LowGameCount = 1 << 1,

    /// <summary>
    /// The <see cref="Match"/> has 1 or more <see cref="Database.Entities.Game"/>s with a <see cref="GameRejectionReason"/>
    /// of <see cref="GameRejectionReason.BeatmapNotPooled"/> outside of the first two <see cref="Database.Entities.Game"/>s
    /// </summary>
    UnexpectedBeatmapsFound = 1 << 2,

    /// <summary>
    /// At least one <see cref="Database.Entities.Player"/> appears in two or more rosters in a <see cref="Match"/>
    /// </summary>
    OverlappingRosters = 1 << 3
}
