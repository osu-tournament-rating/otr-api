using API.DTOs;

namespace API.Services.Interfaces;

public interface ITournamentPlatformStatsService
{
    /// <summary>
    /// Gets various platform-wide stats related to tournaments
    /// </summary>
    public Task<TournamentPlatformStatsDTO> GetAsync();
}
