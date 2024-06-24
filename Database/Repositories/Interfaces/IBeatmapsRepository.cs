using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IBeatmapsRepository : IRepository<Beatmap>
{
    /// <summary>
    /// Returns a beatmap for the given osu! beatmap id
    /// </summary>
    /// <param name="beatmapId">osu! beatmap id</param>
    /// <returns></returns>
    Task<Beatmap?> GetAsync(long beatmapId);

    /// <summary>
    /// Returns the Id (primary key) of a <see cref="Beatmap"/> for a given osu! beatmap id
    /// </summary>
    /// <param name="beatmapId">osu! beatmap id</param>
    /// <returns></returns>
    Task<int?> GetIdAsync(long beatmapId);
}
