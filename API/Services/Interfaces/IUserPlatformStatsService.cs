using API.DTOs;

namespace API.Services.Interfaces;

public interface IUserPlatformStatsService
{
    /// <summary>
    /// Gets various platform-wide <see cref="Database.Entities.User"/> stats
    /// </summary>
    public Task<UserPlatformStatsDTO> GetAsync();
}
