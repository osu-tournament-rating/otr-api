namespace API.DTOs;

/// <summary>
/// Represents platform-wide statistics
/// </summary>
public class PlatformStatsDTO
{
    /// <summary>
    /// Platform-wide tournament stats
    /// </summary>
    public TournamentPlatformStatsDTO TournamentsStats { get; init; } = null!;
}
