namespace Common.Enums.Enums.Verification;

/// <summary>
/// Warnings for irregularities in <see cref="Database.Entities.Game"/> data that don't warrant an automatic
/// <see cref="VerificationStatus"/> of <see cref="VerificationStatus.PreRejected"/>
/// but should have attention drawn to them during manual review
/// </summary>
[Flags]
public enum GameWarningFlags
{
    /// <summary>
    /// The <see cref="Database.Entities.Game"/> has no warnings
    /// </summary>
    None = 0,

    /// <summary>
    /// If the parent <see cref="Database.Entities.Tournament"/> does not have a submitted pool of
    /// <see cref="Database.Entities.Beatmap"/>s, and the <see cref="Database.Entities.Game"/>'s <see cref="Database.Entities.Game.Beatmap"/>
    /// is played only once throughout the entire <see cref="Database.Entities.Tournament"/>
    /// </summary>
    BeatmapUsedOnce = 1 << 0
}
