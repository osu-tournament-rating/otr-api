using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

public class GameScoreDTO
{
    /// <summary>
    /// The id of the Player this score belongs to
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    /// The team the player was on when making this score (red, blue, or none)
    /// </summary>
    public Team Team { get; init; }

    /// <summary>
    /// The points earned
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// The mods applied to this score.
    /// </summary>
    public Mods Mods { get; init; }

    /// <summary>
    /// The number of missed notes
    /// </summary>
    public int Misses { get; init; }

    /// <summary>
    /// The current state of verification
    /// </summary>
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// The current state of processing
    /// </summary>
    public ScoreProcessingStatus ProcessingStatus { get; init; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    public ScoreRejectionReason RejectionReason { get; init; }

    /// <summary>
    /// The accuracy of the score
    /// </summary>
    public double Accuracy { get; init; }

    /// <summary>
    /// All associated admin notes
    /// </summary>
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = new List<AdminNoteDTO>();
}
