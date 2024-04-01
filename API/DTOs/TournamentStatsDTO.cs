namespace API.DTOs;

/// <summary>
/// Represents statistics for a player regarding tournament participation and performance
/// </summary>
public class PlayerTournamentStatsDTO
{
    /// <summary>
    /// Counts of participation in tournaments of differing team sizes for the player
    /// </summary>
    public PlayerTournamentTeamSizeCountDTO TeamSizeCounts { get; set; } = new();

    /// <summary>
    /// List of best tournament performances for the player
    /// </summary>
    public IEnumerable<PlayerTournamentMatchCostDTO> BestPerformances { get; set; } =
        new List<PlayerTournamentMatchCostDTO>();

    /// <summary>
    /// List of worst tournament performances for the player
    /// </summary>
    public IEnumerable<PlayerTournamentMatchCostDTO> WorstPerformances { get; set; } =
        new List<PlayerTournamentMatchCostDTO>();
}
