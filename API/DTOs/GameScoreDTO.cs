using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

public class GameScoreDTO
{
    /// <summary>
    /// The id of the Player this score belongs to
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// The team the player was on when making this score (red, blue, or none)
    /// </summary>
    public Team Team { get; set; }

    /// <summary>
    /// The points earned
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// The mods applied to this score.
    /// </summary>
    public Mods Mods { get; set; }

    /// <summary>
    /// The number of missed notes
    /// </summary>
    public int Misses { get; set; }

    /// <summary>
    /// The current state of verification
    /// </summary>
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// The current state of processing
    /// </summary>
    public ScoreProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    public ScoreRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// The accuracy of the score
    /// </summary>
    public double Accuracy { get; set; }
}
