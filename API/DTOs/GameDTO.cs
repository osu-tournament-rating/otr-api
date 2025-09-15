using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Enums.Verification;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a single game (osu! beatmap) played in a match
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
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
    [EnumDataType(typeof(Ruleset))]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// The scoring type used
    /// </summary>
    [EnumDataType(typeof(ScoringType))]
    public ScoringType ScoringType { get; init; }

    /// <summary>
    /// The team type used
    /// </summary>
    [EnumDataType(typeof(TeamType))]
    public TeamType TeamType { get; init; }

    /// <summary>
    /// The mods enabled
    /// </summary>
    [EnumDataType(typeof(Mods))]
    public Mods Mods { get; init; }

    /// <summary>
    /// Denotes if the mod setting is "free mod"
    /// </summary>
    public bool IsFreeMod { get; init; }

    /// <summary>
    /// The verification status
    /// </summary>
    [EnumDataType(typeof(VerificationStatus))]
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// Warning flags
    /// </summary>
    [EnumDataType(typeof(GameWarningFlags))]
    public GameWarningFlags WarningFlags { get; init; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    [EnumDataType(typeof(GameRejectionReason))]
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
    public BeatmapDTO? Beatmap { get; init; }

    /// <summary>
    /// Win record
    /// </summary>
    public IEnumerable<GameRosterDTO> Rosters { get; init; } = [];

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
