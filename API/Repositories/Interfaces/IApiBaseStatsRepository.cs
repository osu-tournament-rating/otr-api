using API.DTOs;
using API.Enums;
using Database.Entities;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiBaseStatsRepository : IBaseStatsRepository
{
    Task<IEnumerable<BaseStats>> GetLeaderboardAsync(
        int page,
        int pageSize,
        int mode,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO? filter,
        int? playerId
    );

    Task<int> LeaderboardCountAsync(
        int requestQueryMode,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilterDTO requestQueryFilter,
        int? playerId
    );
}
