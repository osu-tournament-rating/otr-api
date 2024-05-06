using API.Entities;

namespace API.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user for the given player id
    /// </summary>
    /// <returns>A user, or null if not found</returns>
    Task<User?> GetByPlayerIdAsync(int playerId);

    /// <summary>
    /// Gets user for the given player id, or create if one doesn't exist
    /// </summary>
    Task<User> GetByPlayerIdOrCreateAsync(int playerId);
}
