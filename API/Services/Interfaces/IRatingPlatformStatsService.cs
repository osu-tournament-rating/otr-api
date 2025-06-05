using API.DTOs;

namespace API.Services.Interfaces;

public interface IRatingPlatformStatsService
{
    /// <summary>
    /// Gets various platform-wide <see cref="Database.Entities.Processor.PlayerRating"/> stats
    /// </summary>
    public Task<RatingPlatformStatsDTO> GetAsync();
}
