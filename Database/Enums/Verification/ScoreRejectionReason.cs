namespace Database.Enums.Verification;

/// <summary>
/// The reason why a score is rejected
/// </summary>
public enum ScoreRejectionReason
{
    /// <summary>
    /// The score is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The score is below the minimum threshold for verification
    /// </summary>
    ScoreBelowMinimum = 1 << 0,

    /// <summary>
    /// The score has invalid mods applied
    /// </summary>
    InvalidMods = 1 << 1,

    /// <summary>
    /// The ruleset of the score does not match the tournament's ruleset
    /// </summary>
    RulesetMismatch = 1 << 2
}
