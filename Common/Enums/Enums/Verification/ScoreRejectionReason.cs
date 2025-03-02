namespace Common.Enums.Enums.Verification;

/// <summary>
/// The reason why a <see cref="Database.Entities.GameScore"/> is rejected
/// </summary>
[Flags]
public enum ScoreRejectionReason
{
    /// <summary>
    /// The <see cref="Database.Entities.GameScore"/> is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="Database.Entities.GameScore"/>'s <see cref="Database.Entities.GameScore.Score"/> is below the minimum threshold
    /// </summary>
    ScoreBelowMinimum = 1 << 0,

    /// <summary>
    /// The <see cref="Database.Entities.GameScore"/> was set with any <see cref="Mods.InvalidMods"/>
    /// </summary>
    InvalidMods = 1 << 1,

    /// <summary>
    /// The <see cref="Database.Entities.GameScore"/>'s <see cref="Ruleset"/> does not match that of the parent <see cref="Database.Entities.Tournament"/>
    /// </summary>
    RulesetMismatch = 1 << 2,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/> the <see cref="Database.Entities.GameScore"/> was set in was rejected
    /// </summary>
    RejectedGame = 1 << 3
}
