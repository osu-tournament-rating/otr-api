using API.DTOs;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class PlatformStatsService(
    ITournamentPlatformStatsService tournamentPlatformStatsService,
    IRatingPlatformStatsService ratingPlatformStatsService) : IPlatformStatsService
{
    public async Task<PlatformStatsDTO> GetAsync() => new()
    {
        TournamentsStats = await tournamentPlatformStatsService.GetAsync(),
        RatingsStats = await ratingPlatformStatsService.GetAsync(),
    };
}

