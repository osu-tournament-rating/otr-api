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
    /// Id of the game
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ruleset for the game
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The scoring type used for the game
    /// </summary>
    public ScoringType ScoringType { get; set; }

    /// <summary>
    /// The team type used for the game
    /// </summary>
    public TeamType TeamType { get; set; }

    /// <summary>
    /// The mods enabled for the game
    /// </summary>
    public Mods Mods { get; set; }

    /// <summary>
    /// osu! id of the game
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// The current state of verification
    /// </summary>
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// The current state of processing
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
    /// The beatmap the game was played on
    /// </summary>
    public BeatmapDTO? Beatmap { get; set; }

    /// <summary>
    /// All admin notes associated with the game
    /// </summary>
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = new List<AdminNoteDTO>();

    /// <summary>
    /// All match scores for the game
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public List<GameScoreDTO> Scores { get; set; } = [];
}
