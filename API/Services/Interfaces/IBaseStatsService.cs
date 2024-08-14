using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using Database.Entities.Processor;

namespace API.Services.Interfaces;

public interface IBaseStatsService
{
    /// <summary>
    ///  Returns a list of all ratings for a player, one for each game mode (if available)
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<IEnumerable<PlayerRatingDTO?>> GetAsync(long osuPlayerId);

    Task<PlayerRatingDTO?> GetAsync(PlayerRating? currentStats, int playerId, int mode);
    Task<int> BatchInsertAsync(IEnumerable<BaseStatsPostDTO> stats);

    Task<IEnumerable<PlayerRatingDTO?>> GetLeaderboardAsync(
        int mode,
        int page,
        int pageSize,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    );

    Task TruncateAsync();
    Task<int> LeaderboardCountAsync(
        int requestQueryMode,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilterDTO requestQueryFilter,
        int? playerId
    );
    Task<LeaderboardFilterDefaultsDTO> LeaderboardFilterDefaultsAsync(
        int requestQueryMode,
        LeaderboardChartType requestQueryChartType
    );

    /// <summary>
    /// See <see cref="IApiBaseStatsRepository.GetHistogramAsync"/>
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    Task<IDictionary<int, int>> GetHistogramAsync(int mode);
}
