// ReSharper disable CommentTypo

using System.Diagnostics.CodeAnalysis;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents a tournament including optional data
/// </summary>
public class TournamentDTO : TournamentCompactDTO
{
    /// <summary>
    /// The current state of verification
    /// </summary>
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// The current state of processing
    /// </summary>
    public TournamentProcessingStatus ProcessingStatus { get; init; }

    /// <summary>
    /// The tournament rejection reason
    /// </summary>
    public TournamentRejectionReason RejectionReason { get; init; }

    /// <summary>
    /// The timestamp of submission for the tournament
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Created { get; init; }

    /// <summary>
    /// The start date of the first match played in the tournament
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// The end date of the last match played in the tournament
    /// </summary>
    public DateTime EndTime { get; init; }

    /// <summary>
    /// All associated match data
    /// </summary>
    /// <remarks>Will be empty for bulk requests such as List</remarks>
    public ICollection<MatchDTO> Matches { get; init; } = new List<MatchDTO>();
}
