namespace Database.Enums.Verification;

/// <summary>
/// The reason why a game is rejected
/// </summary>
[Flags]
public enum GameRejectionReason
{
    /// <summary>
    /// Game is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// osu! API data does not contain scores for the game
    /// </summary>
    NoScores = 1 << 0,

    /// <summary>
    /// Game has invalid mods applied
    /// </summary>
    InvalidMods = 1 << 1,

    /// <summary>
    /// Game's ruleset does not match that of the tournament it belongs to
    /// </summary>
    RulesetMismatch = 1 << 2,

    /// <summary>
    /// Game's scoring type is not equal to 'ScoreV2'
    /// </summary>
    InvalidScoringType = 1 << 3,

    /// <summary>
    /// Game's team type is not TeamVs, even after head to head conversion
    /// </summary>
    InvalidTeamType = 1 << 4,

    /// <summary>
    /// Game's number of verified scores is < 2
    /// </summary>
    NoValidScores = 1 << 5,

    /// <summary>
    /// Game's team size (based on the number of verified scores)
    /// divided by 2 is not equal to the team size of the tournament
    /// </summary>
    TeamSizeMismatch = 1 << 6
}
