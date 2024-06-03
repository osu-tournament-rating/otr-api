using API.DTOs;
using API.Enums;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiMatchRatingStatsRepository : IMatchRatingStatsRepository
{
    /// <summary>
    ///  Get the rating chart for a player
    /// </summary>
    Task<PlayerRatingChartDTO> GetRatingChartAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );

    Task<PlayerRankChartDTO> GetRankChartAsync(
        int playerId,
        int mode,
        LeaderboardChartType chartType,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );
}
