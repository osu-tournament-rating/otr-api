namespace API.DTOs;

/// <summary>
/// Represents platform-wide statistics
/// </summary>
public class PlatformStatsDTO
{
    /// <summary>
    /// Platform-wide tournament stats
    /// </summary>
    public TournamentPlatformStatsDTO TournamentStats { get; init; } = null!;

    /// <summary>
    /// Platform-wide rating stats
    /// </summary>
    public RatingPlatformStatsDTO RatingStats { get; init; } = null!;

    /// <summary>
    /// Platform-wide user stats
    /// </summary>
    public UserPlatformStatsDTO UserStats { get; init; } = null!;
}
