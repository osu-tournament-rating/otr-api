using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IBeatmapsRepository : IRepository<Beatmap>
{
    /// <summary>
    /// Gets a beatmap
    /// </summary>
    /// <param name="osuId">Beatmap osu! id</param>
    /// <returns>A beatmap, or null if not found</returns>
    Task<Beatmap?> GetAsync(long osuId);

    /// <summary>
    /// Gets a beatmap for each given osu! id
    /// </summary>
    /// <param name="osuIds">Beatmap osu! ids</param>
    /// <returns>A list containing a beatmap for each osu! id. If one is not found, it will not be returned.</returns>
    Task<IEnumerable<Beatmap>> GetAsync(IEnumerable<long> osuIds);
}
