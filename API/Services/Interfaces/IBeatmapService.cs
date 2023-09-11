using API.DTOs;
using API.Entities;
using API.Osu;

namespace API.Services.Interfaces;

public interface IBeatmapService : IService<Beatmap>
{
	Task<long> GetBeatmapIdAsync(int id);
	Task<IEnumerable<BeatmapDTO>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds);
	Task<IEnumerable<BeatmapDTO>> GetAllAsync();
	Task<BeatmapDTO?> GetByBeatmapIdAsync(long osuBeatmapId);
	/// <summary>
	/// Returns all beatmap IDs that have not been processed yet with respect to the SR calculation for different mods.
	/// </summary>
	/// <returns>A list of beatmapids that need processing, with the corresponding mod to process</returns>
	Task<HashSet<(int, OsuEnums.Mods)>> GetUnprocessedSrBeatmapIdsAsync();
	Task BulkInsertAsync(IEnumerable<BeatmapModSr> beatmapModSrs);
	Task InsertModSrAsync(BeatmapModSr beatmapModSr);
}