using API.DTOs;
using Common.Enums.Enums;

namespace API.Services.Interfaces;

public interface IPlayerRatingsService
{
    /// <summary>
    ///  Returns a list of all ratings for a player, one for each game ruleset (if available)
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<IEnumerable<PlayerRatingStatsDTO?>> GetAsync(long osuPlayerId);

    Task<PlayerRatingStatsDTO?> GetAsync(int playerId, Ruleset ruleset, bool includeAdjustments);

    Task<LeaderboardDTO> GetLeaderboardAsync(
        LeaderboardRequestQueryDTO request
    );

    /// <summary>
    /// See <see cref="IApiPlayerRatingsRepository.GetHistogramAsync"/>
    /// </summary>
    /// <param name="ruleset"></param>
    /// <returns></returns>
    Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset);
}
