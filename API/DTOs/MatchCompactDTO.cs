using System.ComponentModel.DataAnnotations;
using Common.Enums;
using Common.Enums.Verification;
using JetBrains.Annotations;

namespace API.DTOs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchCompactDTO
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// Title of the lobby
    /// </summary>
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Ruleset
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Start time
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// End time
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Verification status
    /// </summary>
    [EnumDataType(typeof(VerificationStatus))]
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    [EnumDataType(typeof(MatchRejectionReason))]
    public MatchRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Warning flags
    /// </summary>
    [EnumDataType(typeof(MatchWarningFlags))]
    public MatchWarningFlags WarningFlags { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    [EnumDataType(typeof(MatchProcessingStatus))]
    [Obsolete("This property will be removed in a future version. Processing is now handled through event-driven message queue system.")]
    public MatchProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Timestamp of the last time the match was processed
    /// </summary>
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Games played in this match
    /// </summary>
    public ICollection<GameCompactDTO> Games { get; set; } = [];

    /// <summary>
    /// All associated admin notes
    /// </summary>
    public ICollection<AdminNoteDTO> AdminNotes { get; set; } = [];
}
