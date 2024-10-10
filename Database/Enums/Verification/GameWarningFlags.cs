namespace Database.Enums.Verification;

/// <summary>
/// Warnings for irregularities in <see cref="Entities.Game"/> data that don't warrant an automatic
/// <see cref="VerificationStatus"/> of <see cref="VerificationStatus.PreRejected"/>
/// but should have attention drawn to them during manual review
/// </summary>
[Flags]
public enum GameWarningFlags
{
    /// <summary>
    /// The <see cref="Entities.Game"/> has no warnings
    /// </summary>
    None = 0,

    /// <summary>
    /// If the parent <see cref="Entities.Tournament"/> does not have a submitted pool of
    /// <see cref="Entities.Beatmap"/>s, and the <see cref="Entities.Game"/>'s <see cref="Entities.Game.Beatmap"/>
    /// is played only once throughout the entire <see cref="Entities.Tournament"/>
    /// </summary>
    BeatmapUsedOnce = 1 << 0,

    /// <summary>
    /// If the parent <see cref="Entities.Tournament"/> has a submitted pool of <see cref="Entities.Beatmap"/>s,
    /// the <see cref="Entities.Game"/>'s <see cref="Entities.Game.Beatmap"/> is not a part of the pool,
    /// and the <see cref="Entities.Game"/> is not one of the first two in the <see cref="Entities.Match"/>
    /// </summary>
    BeatmapNotInMappool = 1 << 1
}
