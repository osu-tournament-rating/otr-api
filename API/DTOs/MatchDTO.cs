using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents a played match
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class MatchDTO
{
    /// <summary>
    /// Id
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
    public MatchProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Timestamp of the last time the match was processed
    /// </summary>
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// The <see cref="TournamentCompactDTO"/> this match was played in
    /// </summary>
    public TournamentCompactDTO Tournament { get; set; } = null!;

    /// <summary>
    /// The participating <see cref="Player"/>s
    /// </summary>
    public ICollection<PlayerCompactDTO> Players { get; set; } = [];

    /// <summary>
    /// List of games played during the match
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<GameDTO> Games { get; set; } = [];

    /// <summary>
    /// All associated admin notes
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];
}
