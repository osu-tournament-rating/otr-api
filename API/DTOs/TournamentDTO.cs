using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a tournament including optional data
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class TournamentDTO : TournamentCompactDTO
{
    /// <summary>
    /// All associated match data
    /// </summary>
    /// <remarks>Will be empty for bulk requests such as List</remarks>
    public ICollection<MatchCompactDTO> Matches { get; init; } = [];

    /// <summary>
    /// All admin notes associated with the tournament
    /// </summary>
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];

    /// <summary>
    /// All player tournament stats associated with the tournament
    /// </summary>
    public ICollection<PlayerTournamentStatsBaseDTO> PlayerTournamentStats { get; init; } = [];

    /// <summary>
    /// All beatmaps pooled for this tournament
    /// </summary>
    public ICollection<BeatmapDTO> PooledBeatmaps { get; init; } = [];
}
