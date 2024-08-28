using Database.Entities;

namespace Database.Enums.Verification;

/// <summary>
/// The reason why a <see cref="Entities.Game"/> is rejected
/// </summary>
[Flags]
public enum GameRejectionReason
{
    /// <summary>
    /// The <see cref="Entities.Game"/> is not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="Entities.Game"/>'s osu! API data did not contain any <see cref="Entities.GameScore"/>s
    /// </summary>
    NoScores = 1 << 0,

    /// <summary>
    /// The <see cref="Entities.Game"/> has invalid mods applied
    /// </summary>
    InvalidMods = 1 << 1,

    /// <summary>
    /// The <see cref="Entities.Game"/>'s <see cref="Ruleset"/> does not match that of the parent <see cref="Entities.Tournament"/>
    /// </summary>
    RulesetMismatch = 1 << 2,

    /// <summary>
    /// The <see cref="Entities.Game"/>'s <see cref="ScoringType"/> is not <see cref="ScoringType.ScoreV2"/>
    /// </summary>
    InvalidScoringType = 1 << 3,

    /// <summary>
    /// The <see cref="Entities.Game"/>'s <see cref="TeamType"/> is not <see cref="TeamType.TeamVs"/>
    /// </summary>
    InvalidTeamType = 1 << 4,

    /// <summary>
    /// The <see cref="Entities.Game"/>'s <see cref="TeamType"/> is not <see cref="TeamType.TeamVs"/>
    /// and attempting <see cref="TeamType.TeamVs"/> conversion was not successful
    /// </summary>
    FailedTeamVsConversion = 1 << 5,

    /// <summary>
    /// The <see cref="Entities.Game"/>'s number of <see cref="Entities.Game.Scores"/> with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/> is &lt; 2
    /// </summary>
    NoValidScores = 1 << 6,

    /// <summary>
    /// The <see cref="Entities.Game"/>'s number of <see cref="Entities.Game.Scores"/> with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/> or <see cref="VerificationStatus.PreVerified"/> divided by 2 is
    /// not equal to the <see cref="Tournament.LobbySize"/> of the parent <see cref="Entities.Tournament"/>
    /// </summary>
    LobbySizeMismatch = 1 << 7,

    /// <summary>
    /// The <see cref="Entities.Game"/>'s <see cref="Entities.Game.EndTime"/> could not be determined
    /// </summary>
    NoEndTime = 1 << 8
}
