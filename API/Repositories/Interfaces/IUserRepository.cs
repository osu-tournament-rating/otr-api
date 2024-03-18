using API.Entities;

namespace API.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Get user by osu! player id
    /// </summary>
    /// <param name="osuId">osu! account id</param>
    /// <returns></returns>
    Task<User?> GetByOsuIdAsync(int osuId);

    /// <summary>
    /// Get user by osu! player id, or create if one doesn't exist
    /// </summary>
    /// <param name="osuId">osu! account id</param>
    /// <returns></returns>
    Task<User> GetOrCreateAsync(int osuId);
}
