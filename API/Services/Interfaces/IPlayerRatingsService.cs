using API.DTOs;
using Common.Enums;

namespace API.Services.Interfaces;

public interface IPlayerRatingsService
{
    Task<PlayerRatingStatsDTO?> GetAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null, bool includeAdjustments = false);

    /// <summary>
    /// Get rating stats for multiple players in a single query
    /// </summary>
    /// <param name="playerIds">The player IDs to fetch</param>
    /// <param name="ruleset">The ruleset to fetch ratings for</param>
    /// <param name="dateMin">Optional minimum date filter</param>
    /// <param name="dateMax">Optional maximum date filter</param>
    /// <param name="includeAdjustments">Whether to include rating adjustments</param>
    /// <returns>Dictionary mapping player IDs to their rating stats (null if no rating exists)</returns>
    Task<Dictionary<int, PlayerRatingStatsDTO?>> GetAsync(IEnumerable<int> playerIds, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null, bool includeAdjustments = false);
}
