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
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    public MatchRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    public MatchProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Timestamp of the last time the match was processed
    /// </summary>
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// List of games played during the match
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<GameDTO> Games { get; set; } = new List<GameDTO>();

    /// <summary>
    /// The admin notes attached to this match. Null if the user is not an admin.
    /// Empty collection if the user is an admin but there are no notes.
    /// </summary>
    public ICollection<MatchAdminNoteDTO>? AdminNotes { get; set; }
}
