using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a played match
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchDTO : MatchCompactDTO
{
    /// <summary>
    /// The <see cref="TournamentCompactDTO"/> this match was played in
    /// </summary>
    public TournamentCompactDTO Tournament { get; set; } = null!;

    /// <summary>
    /// The participating <see cref="Player"/>s
    /// </summary>
    public ICollection<PlayerCompactDTO> Players { get; set; } = [];

    /// <summary>
    /// Match stats for each participant
    /// </summary>
    public ICollection<PlayerMatchStatsDTO> PlayerMatchStats { get; set; } = [];

    /// <summary>
    /// Rating adjustments for each participant
    /// </summary>
    public ICollection<RatingAdjustmentDTO> RatingAdjustments { get; set; } = [];

    /// <summary>
    /// Match win record information
    /// </summary>
    public MatchWinRecordDTO? MatchWinRecord { get; set; }

    /// <summary>
    /// List of games played during the match
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public new ICollection<GameDTO> Games { get; set; } = [];

    /// <summary>
    /// All associated admin notes
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public new ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];
}
