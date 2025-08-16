using System.ComponentModel.DataAnnotations;
using Common.Enums;
using Common.Enums.Verification;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents essential game information without nested data
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class GameCompactDTO
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; init; }

    /// <summary>
    /// The ruleset
    /// </summary>
    [EnumDataType(typeof(Ruleset))]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// The verification status
    /// </summary>
    [EnumDataType(typeof(VerificationStatus))]
    public VerificationStatus VerificationStatus { get; init; }


    /// <summary>
    /// Warning flags
    /// </summary>
    [EnumDataType(typeof(GameWarningFlags))]
    public GameWarningFlags WarningFlags { get; init; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    [EnumDataType(typeof(GameRejectionReason))]
    public GameRejectionReason RejectionReason { get; init; }

    /// <summary>
    /// Timestamp of the beginning of the game
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// Timestamp of the end of the game
    /// </summary>
    public DateTime? EndTime { get; init; }
}
