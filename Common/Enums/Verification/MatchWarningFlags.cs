namespace Common.Enums.Verification;

/// <summary>
/// Warnings for irregularities in <see cref="Database.Entities.Match"/> data that don't warrant an automatic
/// <see cref="VerificationStatus"/> of <see cref="VerificationStatus.PreRejected"/>
/// but should have attention drawn to them during manual review
/// </summary>
[Flags]
public enum MatchWarningFlags
{
    /// <summary>
    /// The <see cref="Database.Entities.Match"/> has no warnings
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/>'s <see cref="Database.Entities.Match.Name"/> does not follow common tournament
    /// lobby title conventions
    /// </summary>
    UnexpectedNameFormat = 1 << 0,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/>'s number of <see cref="Database.Entities.Match.Games"/> is exactly 3 or 4
    /// </summary>
    LowGameCount = 1 << 1,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/> has 1 or more <see cref="Database.Entities.Game"/>s with a <see cref="GameRejectionReason"/>
    /// of <see cref="GameRejectionReason.BeatmapNotPooled"/> outside of the first two <see cref="Database.Entities.Game"/>s
    /// </summary>
    UnexpectedBeatmapsFound = 1 << 2,

    /// <summary>
    /// At least one <see cref="Database.Entities.Player"/> appears in two or more rosters in a <see cref="Database.Entities.Match"/>
    /// </summary>
    OverlappingRosters = 1 << 3,
}
