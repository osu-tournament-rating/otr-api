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
        TournamentStats = await tournamentPlatformStatsService.GetAsync(),
        RatingStats = await ratingPlatformStatsService.GetAsync(),
        UserStats = await userPlatformStatsService.GetAsync()
    };
}

