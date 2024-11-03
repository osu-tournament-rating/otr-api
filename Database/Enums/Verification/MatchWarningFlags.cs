namespace Database.Enums.Verification;

/// <summary>
/// Warnings for irregularities in <see cref="Entities.Match"/> data that don't warrant an automatic
/// <see cref="VerificationStatus"/> of <see cref="VerificationStatus.PreRejected"/>
/// but should have attention drawn to them during manual review
/// </summary>
[Flags]
public enum MatchWarningFlags
{
    /// <summary>
    /// The <see cref="Entities.Match"/> has no warnings
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="Entities.Match"/>'s <see cref="Entities.Match.Name"/> does not follow common tournament
    /// lobby title conventions
    /// </summary>
    UnexpectedTitleFormat = 1 << 0,

    /// <summary>
    /// The <see cref="Entities.Match"/>'s number of <see cref="Entities.Match.Games"/> is exactly 3 or 4
    /// </summary>
    LowGameCount = 1 << 1,

    /// <summary>
    /// The <see cref="Match"/> has 1 or more <see cref="Game"/>s with a <see cref="GameRejectionReason"/>
    /// of <see cref="GameRejectionReason.BeatmapNotPooled"/> outside of the first two <see cref="Game"/>s
    /// </summary>
    UnexpectedBeatmapsFound = 1 << 2
}
