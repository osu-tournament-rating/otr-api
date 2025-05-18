using API.DTOs;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class PlatformStatsService(
    ITournamentPlatformStatsService tournamentPlatformStatsService,
    IRatingPlatformStatsService ratingPlatformStatsService,
    IUserPlatformStatsService userPlatformStatsService) : IPlatformStatsService
{
    public async Task<PlatformStatsDTO> GetAsync() => new()
    {
        TournamentsStats = await tournamentPlatformStatsService.GetAsync(),
        RatingsStats = await ratingPlatformStatsService.GetAsync(),
        UsersStats = await userPlatformStatsService.GetAsync(),
    };
}

