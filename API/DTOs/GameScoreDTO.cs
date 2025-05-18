using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using API.Utilities.DataAnnotations;
using Common.Enums;
using Common.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents a single score set in a game
/// </summary>
public class GameScoreDTO
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Id of the Player that set the score
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    /// Ruleset the score was set in
    /// </summary>
    [EnumDataType(typeof(Ruleset))]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Team the Player was on
    /// </summary>
    [EnumDataType(typeof(Team))]
    public Team Team { get; init; }

    /// <summary>
    /// Letter grade
    /// </summary>
    [EnumDataType(typeof(ScoreGrade))]
    public ScoreGrade Grade { get; init; }

    /// <summary>
    /// Total score
    /// </summary>
    [Positive(allowZero: true)]
    public int Score { get; init; }

    /// <summary>
    /// Placement of the score compared to all others in the same game
    /// </summary>
    [Positive(allowZero: true)]
    public int Placement { get; init; }

    /// <summary>
    /// Max combo
    /// </summary>
    [Positive(allowZero: true)]
    public int MaxCombo { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of 50
    /// </summary>
    [Positive(allowZero: true)]
    public int Count50 { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of 100
    /// </summary>
    [Positive(allowZero: true)]
    public int Count100 { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of 300
    /// </summary>
    [Positive(allowZero: true)]
    public int Count300 { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of Katu
    /// </summary>
    [Positive(allowZero: true)]
    public int CountKatu { get; init; }

    /// <summary>
    /// Count of notes hit with a judgement of Geki
    /// </summary>
    [Positive(allowZero: true)]
    public int CountGeki { get; init; }

    /// <summary>
    /// Count of missed notes
    /// </summary>
    [Positive(allowZero: true)]
    public int CountMiss { get; init; }

    /// <summary>
    /// Applied mods
    /// </summary>
    [EnumDataType(typeof(Mods))]
    public Mods Mods { get; init; }

    /// <summary>
    /// Accuracy
    /// </summary>
    public double Accuracy { get; init; }

    /// <summary>
    /// The current state of verification
    /// </summary>
    [EnumDataType(typeof(VerificationStatus))]
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// The current state of processing
    /// </summary>
    [EnumDataType(typeof(ScoreProcessingStatus))]
    public ScoreProcessingStatus ProcessingStatus { get; init; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    [EnumDataType(typeof(ScoreRejectionReason))]
    public ScoreRejectionReason RejectionReason { get; init; }

    /// <summary>
    /// All associated admin notes
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];
}
