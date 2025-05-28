using Common.Enums;

namespace API.DTOs;

/// <summary>
/// Represents platform-wide <see cref="Database.Entities.Processor.PlayerRating"/> stats
/// </summary>
public class RatingPlatformStatsDTO
{
    /// <summary>
    /// For each ruleset, a map of rating 'buckets' (i.e. 100, 125, 150, etc. rating)s
    /// to the number of <see cref="Database.Entities.Player"/>s in that 'bucket'
    /// </summary>
    public Dictionary<Ruleset, Dictionary<int, int>> RatingsByRuleset { get; init; } = new();
}
