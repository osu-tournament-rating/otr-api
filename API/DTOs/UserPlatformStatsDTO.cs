namespace API.DTOs;

/// <summary>
/// Represents platform-wide <see cref="Database.Entities.User"/> stats
/// </summary>
public class UserPlatformStatsDTO
{
    /// <summary>
    /// Map of dates to the total number of <see cref="Database.Entities.User"/>s being registered at that point of time
    /// </summary>
    public Dictionary<DateTime, int> AccumulatedCountByDate { get; init; } = new();
}
