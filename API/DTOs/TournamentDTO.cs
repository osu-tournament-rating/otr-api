namespace API.DTOs;

/// <summary>
/// Represents a tournament including optional data
/// </summary>
public class TournamentDTO : TournamentCompactDTO
{
    /// <summary>
    /// All associated match data
    /// </summary>
    /// <remarks>Will be empty for bulk requests such as List</remarks>
    public ICollection<MatchDTO> Matches { get; init; } = [];

    /// <summary>
    /// All admin notes associated with the tournament
    /// </summary>
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];

    /// <summary>
    /// All player tournament stats associated with the tournament
    /// </summary>
    public ICollection<PlayerTournamentStatsBaseDTO> PlayerTournamentStats { get; init; } = [];
}
