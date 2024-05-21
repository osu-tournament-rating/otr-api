using API.Entities;

namespace API.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user for the given player id
    /// </summary>
    /// <returns>A user, or null if not found</returns>
    Task<User?> GetByPlayerIdAsync(int playerId, bool loadSettings = false);

    /// <summary>
    /// Gets a user for the given player id, or create if one doesn't exist
    /// </summary>
    Task<User> GetByPlayerIdOrCreateAsync(int playerId);

    /// <summary>
    /// Gets a user's player id for the given id
    /// </summary>
    /// <returns> A player id, or null if a user was not found or the user has no player entry </returns>
    Task<int?> GetPlayerIdAsync(int id);

    /// <summary>
    /// Gets a list of OAuth clients owned by the user for the given id
    /// </summary>
    Task<IEnumerable<OAuthClient>> GetClientsAsync(int id);

    /// <summary>
    /// Gets a list of matches submitted by the user for the given id
    /// </summary>
    Task<IEnumerable<Match>> GetSubmissionsAsync(int id);
}
