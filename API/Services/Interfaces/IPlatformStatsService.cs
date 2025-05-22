using API.DTOs;

namespace API.Services.Interfaces;

public interface IPlatformStatsService
{
    /// <summary>
    /// Gets various platform-wide stats
    /// </summary>
    Task<PlatformStatsDTO> GetAsync();
}
