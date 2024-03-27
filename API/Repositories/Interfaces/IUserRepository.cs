using API.Entities;

namespace API.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Get user by player id
    /// </summary>
    /// <param name="playerId">player id (primary key)</param>
    /// <returns></returns>
    Task<User?> GetByPlayerIdAsync(int playerId);

    /// <summary>
    /// Get user by player id, or create if one doesn't exist
    /// </summary>
    /// <param name="playerId">player id (primary key)</param>
    /// <returns></returns>
    Task<User> GetByPlayerIdOrCreateAsync(int playerId);
}
