using API.DTOs;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiMatchRatingStatsRepository : IMatchRatingStatsRepository
{
    /// <summary>
    ///  Get the rating chart for a player
    /// </summary>
    Task<PlayerRatingChartDTO> GetRatingChartAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );
}
