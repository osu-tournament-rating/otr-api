using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IBeatmapsetsRepository : IRepository<Beatmapset>
{
    /// <summary>
    /// Gets a beatmapset with its creator and beatmaps
    /// </summary>
    /// <param name="osuId">Beatmapset osu! id</param>
    /// <returns>A beatmapset with related data, or null if not found</returns>
    Task<Beatmapset?> GetWithDetailsAsync(long osuId);

    /// <summary>
    /// Marks all beatmaps in a beatmapset as having no data
    /// </summary>
    /// <param name="beatmapsetId">Beatmapset id</param>
    /// <returns>Number of beatmaps updated</returns>
    Task<int> MarkBeatmapsAsNoDataAsync(int beatmapsetId);
}
