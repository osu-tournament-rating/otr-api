using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using Database.Entities.Processor;
using Database.Enums;

namespace API.Services.Interfaces;

public interface IPlayerRatingsService
{
    /// <summary>
    ///  Returns a list of all ratings for a player, one for each game ruleset (if available)
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<IEnumerable<PlayerRatingStatsDTO?>> GetAsync(long osuPlayerId);

    Task<PlayerRatingStatsDTO?> GetAsync(PlayerRating? currentStats, int playerId, Ruleset ruleset);

    Task<IEnumerable<PlayerRatingStatsDTO?>> GetLeaderboardAsync(
        Ruleset ruleset,
        int page,
        int pageSize,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    );

    Task<int> LeaderboardCountAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilterDTO requestQueryFilter,
        int? playerId
    );

    Task<LeaderboardFilterDefaultsDTO> LeaderboardFilterDefaultsAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType requestQueryChartType
    );

    /// <summary>
    /// See <see cref="IApiPlayerRatingsRepository.GetHistogramAsync"/>
    /// </summary>
    /// <param name="ruleset"></param>
    /// <returns></returns>
    Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset);
}
