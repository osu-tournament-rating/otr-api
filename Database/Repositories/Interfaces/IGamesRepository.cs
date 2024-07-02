using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IGamesRepository : IRepository<Game>
{
    /// <summary>
    /// Gets a game for the given osu! id
    /// </summary>
    /// <param name="osuId">Game osu! id</param>
    /// <returns>A game, or null if not found</returns>
    Task<Game?> GetAsync(long osuId);
}
