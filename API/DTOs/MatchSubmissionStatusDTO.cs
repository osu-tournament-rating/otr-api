using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Represents the status of a submitted match
/// </summary>
public class MatchSubmissionStatusDTO
{
    /// <summary>
    /// Id of the match
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// osu! match id of the match
    /// </summary>
    public long MatchId { get; set; }

    /// <summary>
    /// Lobby title of the match
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Current verification status of the match
    /// </summary>
    public MatchVerificationStatus? VerificationStatus { get; set; }

    /// <summary>
    /// Date that the match was submitted
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Date that the match was last updated
    /// </summary>
    public DateTime? Updated { get; set; }
}
