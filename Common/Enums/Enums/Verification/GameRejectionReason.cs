namespace Common.Enums.Enums.Verification;

/// <summary>
/// The reason why a <see cref="Database.Entities.Game"/> is rejected
/// </summary>
[Flags]
public enum GameRejectionReason
{
    /// <summary>
    /// The <see cref="Database.Entities.Game"/> is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/>'s osu! API data did not contain any <see cref="Database.Entities.GameScore"/>s
    /// </summary>
    NoScores = 1 << 0,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/> has invalid mods applied
    /// </summary>
    InvalidMods = 1 << 1,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/>'s <see cref="Ruleset"/> does not match that of the parent <see cref="Database.Entities.Tournament"/>
    /// </summary>
    RulesetMismatch = 1 << 2,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/>'s <see cref="ScoringType"/> is not <see cref="ScoringType.ScoreV2"/>
    /// </summary>
    InvalidScoringType = 1 << 3,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/>'s <see cref="TeamType"/> is not <see cref="TeamType.TeamVs"/>
    /// </summary>
    InvalidTeamType = 1 << 4,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/>'s <see cref="TeamType"/> is not <see cref="TeamType.TeamVs"/>
    /// and attempting <see cref="TeamType.TeamVs"/> conversion was not successful
    /// </summary>
    FailedTeamVsConversion = 1 << 5,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/>'s number of <see cref="Database.Entities.Game.Scores"/> with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/> is &lt; 2
    /// </summary>
    NoValidScores = 1 << 6,

    /// <summary>
    /// The game's number of scores with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/> divided by 2 is
    /// not equal to the lobby size of the parent tournament in case of head-to-head games.
    /// Or the number of validated team red and blue scores is not valid in case of team games.
    /// </summary>
    LobbySizeMismatch = 1 << 7,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/>'s <see cref="Database.Entities.Game.EndTime"/> could not be determined
    /// </summary>
    NoEndTime = 1 << 8,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/> the <see cref="Database.Entities.Game"/> was played in was rejected
    /// </summary>
    RejectedMatch = 1 << 9,

    /// <summary>
    /// The <see cref="Database.Entities.Tournament"/> has a known collection of PooledBeatmaps
    /// and the <see cref="Database.Entities.Beatmap"/> played in the <see cref="Database.Entities.Game"/> is not present
    /// in said collection
    /// </summary>
    BeatmapNotPooled = 1 << 10
}
