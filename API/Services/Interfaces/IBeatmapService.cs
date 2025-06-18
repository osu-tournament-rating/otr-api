using API.DTOs;
using JetBrains.Annotations;

namespace API.Services.Interfaces;

public interface IBeatmapService
{
    /// <summary>
    /// List all beatmaps
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<BeatmapDTO>> ListAsync();

    /// <summary>
    /// Get a beatmap by primary key
    /// </summary>
    /// <param name="id">primary key</param>
    /// <returns></returns>
    Task<BeatmapDTO?> GetAsync(int id);

    /// <summary>
    /// Get a beatmap by osu! beatmapId
    /// </summary>
    /// <param name="beatmapId">osu! beatmapId</param>
    /// <returns></returns>
    Task<BeatmapDTO?> GetAsync(long beatmapId);

    /// <summary>
    /// Dynamically searches for a beatmap via the following, in order of priority:
    ///
    /// If the key can be parsed as an integer:
    /// - The beatmap id (primary key)
    ///
    /// If the key can be parsed as a long:
    /// - The beatmap id (osu! id)
    /// </summary>
    /// <param name="key">The dynamic key of the beatmap to look for</param>
    /// <returns></returns>
    Task<BeatmapDTO?> GetVersatileAsync(long key);
}
