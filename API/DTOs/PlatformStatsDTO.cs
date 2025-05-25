namespace API.DTOs;

/// <summary>
/// Represents platform-wide statistics
/// </summary>
public class PlatformStatsDTO // TODO: use IDictionary instead of Dictionary
{
    /// <summary>
    /// Platform-wide tournament stats
    /// </summary>
    public TournamentPlatformStatsDTO TournamentsStats { get; init; } = null!;

    /// <summary>
    /// Platform-wide rating stats
    /// </summary>
    public RatingPlatformStatsDTO RatingsStats { get; init; } = null!;
}
