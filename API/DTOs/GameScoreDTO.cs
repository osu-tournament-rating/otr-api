using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents a single score set in a game
/// </summary>
public class GameScoreDTO
{
    /// <summary>
    /// Id of the Player that set the score
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    /// Ruleset the score was set in
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Team the Player was on
    /// </summary>
    public Team Team { get; init; }

    /// <summary>
    /// Letter grade
    /// </summary>
    public ScoreGrade Grade { get; init; }

    /// <summary>
    /// Total score
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Placement of the score compared to all others in the same game
    /// </summary>
    public int Placement { get; init; }

    /// <summary>
    /// Max combo
    /// </summary>
    public int MaxCombo { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of 50
    /// </summary>
    public int Count50 { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of 100
    /// </summary>
    public int Count100 { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of 300
    /// </summary>
    public int Count300 { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of Katu
    /// </summary>
    public int CountKatu { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of Geki
    /// </summary>
    public int CountGeki { get; init; }

    /// <summary>
    /// Count of missed notes
    /// </summary>
    public int CountMiss { get; init; }

    /// <summary>
    /// Applied mods
    /// </summary>
    public Mods Mods { get; init; }

    /// <summary>
    /// Accuracy
    /// </summary>
    public double Accuracy { get; init; }

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
    /// All associated admin notes
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];
}
