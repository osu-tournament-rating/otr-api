using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Database.Enums.Verification;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a single game (osu! beatmap) played in a match
/// </summary>
public class GameDTO
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; init; }

    /// <summary>
    /// The ruleset
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// The scoring type used
    /// </summary>
    public ScoringType ScoringType { get; init; }

    /// <summary>
    /// The team type used
    /// </summary>
    public TeamType TeamType { get; init; }

    /// <summary>
    /// The mods enabled
    /// </summary>
    public Mods Mods { get; init; }

    /// <summary>
    /// Denotes if the mod setting is "free mod"
    /// </summary>
    public bool IsFreeMod { get; init; }

    /// <summary>
    /// The verification status
    /// </summary>
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// The processing status
    /// </summary>
    public GameProcessingStatus ProcessingStatus { get; init; }

    /// <summary>
    /// Warning flags
    /// </summary>
    public GameWarningFlags WarningFlags { get; init; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    public GameRejectionReason RejectionReason { get; init; }

    /// <summary>
    /// Timestamp of the beginning of the game
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// Timestamp of the end of the game
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// The beatmap played
    /// </summary>
    public BeatmapDTO Beatmap { get; init; } = null!;

    /// <summary>
    /// Win record
    /// </summary>
    public GameWinRecordDTO? WinRecord { get; init; }

    /// <summary>
    /// All participating players
    /// </summary>
    /// <remarks>
    /// Will only be populated if the game is the highest order of entity requested
    /// </remarks>
    [UsedImplicitly]
    public ICollection<PlayerCompactDTO> Players { get; set; } = [];

    /// <summary>
    /// All associated admin notes
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];

    /// <summary>
    /// All match scores
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<GameScoreDTO> Scores { get; init; } = [];
}
