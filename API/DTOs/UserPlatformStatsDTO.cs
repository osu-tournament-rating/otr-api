namespace API.DTOs;

/// <summary>
/// Represents platform-wide <see cref="Database.Entities.User"/> stats
/// </summary>
public class UserPlatformStatsDTO
{
    /// <summary>
    /// Map of dates to the total number of registered <see cref="Database.Entities.User"/>s at that time
    /// </summary>
    /// <remarks>One entry per day beginning from the date of the first registered user</remarks>
    public IDictionary<DateTime, int> SumByDate { get; init; } = new Dictionary<DateTime, int>();
}
