using API.DTOs;
using API.Services.Interfaces;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class RatingPlatformStatsService(IPlayerRatingsRepository playerRatingsRepository) : IRatingPlatformStatsService
{
    public async Task<RatingPlatformStatsDTO> GetAsync() => new()
    {
        RatingsByRuleset = await playerRatingsRepository.GetHistogramAsync()
    };
}
