using System.Diagnostics.CodeAnalysis;

namespace API.DTOs;

/// <summary>
/// Represents statistics for a player regarding tournament participation and performance
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PlayerTournamentPerformanceDTO
{
    /// <summary>
    /// Counts of participation in tournaments of differing formats for the player
    /// </summary>
    public PlayerTournamentFormatCountDTO TournamentFormatCounts { get; set; } = new();

    /// <summary>
    /// List of best tournament performances for the player
    /// </summary>
    public IEnumerable<PlayerTournamentStatsDTO> BestPerformances { get; set; } =
        [];

    /// <summary>
    /// List of recent tournament performances for the player
    /// </summary>
    public IEnumerable<PlayerTournamentStatsDTO> RecentPerformances { get; set; } =
        [];
}
