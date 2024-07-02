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
}
