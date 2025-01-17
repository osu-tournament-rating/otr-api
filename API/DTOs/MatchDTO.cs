using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents a played match
/// </summary>
public class MatchDTO
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; init; }

    /// <summary>
    /// Title of the lobby
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Ruleset
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Start time
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// End time
    /// </summary>
    public DateTime EndTime { get; init; }

    /// <summary>
    /// Verification status
    /// </summary>
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    public MatchRejectionReason RejectionReason { get; init; }

    /// <summary>
    /// Warning flags
    /// </summary>
    public MatchWarningFlags WarningFlags { get; init; }

    /// <summary>
    /// Processing status
    /// </summary>
    public MatchProcessingStatus ProcessingStatus { get; init; }

    /// <summary>
    /// Timestamp of the last time the match was processed
    /// </summary>
    public DateTime LastProcessingDate { get; init; }

    /// <summary>
    /// The <see cref="TournamentCompactDTO"/> this match was played in
    /// </summary>
    public TournamentCompactDTO Tournament { get; init; } = null!;

    /// <summary>
    /// The participating <see cref="Player"/>s
    /// </summary>
    public ICollection<PlayerCompactDTO> Players { get; set; } = [];

    /// <summary>
    /// Team rosters
    /// </summary>
    /// <remarks>Only populated for fully processed and verified matches</remarks>
    public ICollection<RosterDTO> Rosters { get; init; } = [];

    /// <summary>
    /// List of games played during the match
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<GameDTO> Games { get; init; } = [];

    /// <summary>
    /// All associated admin notes
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];
}
