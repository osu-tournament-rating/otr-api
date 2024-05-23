using System.Diagnostics.CodeAnalysis;

namespace API.DTOs;

/// <summary>
/// Represents statistics for a player regarding tournament participation and performance
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
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
    /// List of recent tournament performances for the player
    /// </summary>
    public IEnumerable<PlayerTournamentMatchCostDTO> RecentPerformances { get; set; } =
        new List<PlayerTournamentMatchCostDTO>();
}
