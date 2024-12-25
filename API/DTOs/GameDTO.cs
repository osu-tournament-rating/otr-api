using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents a single game (osu! beatmap) played in a match
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class GameDTO
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ruleset
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The scoring type used
    /// </summary>
    public ScoringType ScoringType { get; set; }

    /// <summary>
    /// The team type used
    /// </summary>
    public TeamType TeamType { get; set; }

    /// <summary>
    /// The mods enabled
    /// </summary>
    public Mods Mods { get; set; }

    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// The verification status
    /// </summary>
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// The processing status
    /// </summary>
    public GameProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Warning flags
    /// </summary>
    public GameWarningFlags WarningFlags { get; set; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    public GameRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Timestamp of the beginning of the game
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Timestamp of the end of the game
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// The beatmap played
    /// </summary>
    public BeatmapDTO? Beatmap { get; set; }

    /// <summary>
    /// All associated admin notes
    /// </summary>
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];

    /// <summary>
    /// All match scores
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public List<GameScoreDTO> Scores { get; set; } = [];
}
