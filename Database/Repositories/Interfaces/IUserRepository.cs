using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user for the given player id
    /// </summary>
    /// <param name="playerId">Id of the player</param>
    /// <param name="loadSettings">If true, includes the <see cref="User.Settings"/> of the user</param>
    /// <returns>A user, or null if not found</returns>
    Task<User?> GetByPlayerIdAsync(int playerId, bool loadSettings = false);

    /// <summary>
    /// Gets a user for the given player id, or create if one doesn't exist
    /// </summary>
    /// <param name="playerId">Id of the player</param>
    Task<User> GetByPlayerIdOrCreateAsync(int playerId);

    /// <summary>
    /// Gets a user's player id
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <returns> A player id, or null if a user was not found or the user has no player entry </returns>
    Task<int?> GetPlayerIdAsync(int id);

    /// <summary>
    /// Gets a user's OAuth clients
    /// </summary>
    /// <param name="id">Id of the user</param>
    Task<IEnumerable<OAuthClient>> GetClientsAsync(int id);

    /// <summary>
    /// Gets a user's submitted matches
    /// </summary>
    /// <param name="id">Id of the user</param>
    Task<IEnumerable<Match>> GetSubmissionsAsync(int id);

    /// <summary>
    /// Gets a map of dates to the total number of <see cref="Database.Entities.User"/>s
    /// being registered at that point of time
    /// </summary>
    Task<Dictionary<DateTime, int>> GetAccumulatedDailyCountsAsync();
}
