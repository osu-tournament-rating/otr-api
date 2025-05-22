using API.DTOs;

namespace API.Services.Interfaces;

public interface ITournamentPlatformStatsService
{
    /// <summary>
    /// Gets various platform-wide tournament stats
    /// </summary>
    public Task<TournamentPlatformStatsDTO> GetAsync();
}
