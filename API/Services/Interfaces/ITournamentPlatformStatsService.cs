using API.DTOs;

namespace API.Services.Interfaces;

public interface ITournamentPlatformStatsService
{
    /// <summary>
    /// Gets various platform-wide <see cref="Database.Entities.Tournament"/> stats
    /// </summary>
    public Task<TournamentPlatformStatsDTO> GetAsync();
}
