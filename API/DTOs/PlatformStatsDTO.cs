namespace API.DTOs;

/// <summary>
/// Represents platform-wide statistics
/// </summary>
public class PlatformStatsDTO
{
    /// <summary>
    /// Statistics on all <see cref="Database.Entities.Tournament"/>s existing in the system
    /// </summary>
    public TournamentPlatformStatsDTO TournamentsStats { get; init; } = null!;
}
