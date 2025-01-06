using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IGamesRepository : IRepository<Game>
{
    /// <summary>
    /// Gets a game for the given id including all navigation properties.
    /// </summary>
    /// <remarks>The game and included navigations will not be tracked in the context</remarks>
    /// <param name="id">Game id</param>
    /// <param name="verified">Whether the game and all child navigations must be verified</param>
    public Task<Game?> GetAsync(int id, bool verified);
}
