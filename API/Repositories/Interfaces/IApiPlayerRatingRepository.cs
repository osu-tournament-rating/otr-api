using API.DTOs;
using API.Enums;
using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiPlayerRatingRepository : IBaseStatsRepository
{
    Task<IEnumerable<PlayerRating>> GetLeaderboardAsync(
        int page,
        int pageSize,
        Ruleset ruleset,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO? filter,
        int? playerId
    );

    Task<int> LeaderboardCountAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilterDTO requestQueryFilter,
        int? playerId
    );
}
