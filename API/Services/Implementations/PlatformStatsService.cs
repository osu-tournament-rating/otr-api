using API.DTOs;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class PlatformStatsService(ITournamentPlatformStatsService tournamentsStatsService) : IPlatformStatsService
{
    public async Task<PlatformStatsDTO> GetAsync() => new()
    {
        TournamentsStats = await tournamentsStatsService.GetAsync(),
    };
}

