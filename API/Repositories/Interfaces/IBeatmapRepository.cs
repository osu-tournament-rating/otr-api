using API.Entities;
using API.Osu;

namespace API.Repositories.Interfaces;

public interface IBeatmapRepository : IRepository<Beatmap>
{
	Task<long> GetBeatmapIdAsync(int id);
	Task<Beatmap?> GetByOsuIdAsync(long osuBeatmapId);

	/// <summary>
	///  Returns all beatmap IDs that have not been processed yet with respect to the SR calculation for different mods.
	/// </summary>
	/// <returns>A list of beatmapids that need processing, with the corresponding mod to process</returns>
	Task<HashSet<(int, OsuEnums.Mods)>> GetUnprocessedSrBeatmapIdsAsync(); // TODO: Remove this
	Task BulkInsertAsync(IEnumerable<BeatmapModSr> beatmapModSrs);         // TODO: Remove this
	Task<double> GetDoubleTimeSrAsync(int beatmapId);                      // TODO: Remove this
	Task<double> GetHardRockSrAsync(int beatmapId);                        // TODO: Remove this
	Task<double> GetEasySrAsync(int beatmapId);                            // TODO: Remove this
	Task<double> GetHalfTimeSrAsync(int beatmapId);                        // TODO: Remove this
	Task CreateIfNotExistsAsync(IEnumerable<long> beatmapIds);
	Task<int?> GetIdByBeatmapIdAsync(long gameBeatmapId);
	Task<IEnumerable<Beatmap>> GetAllAsync();
	/// <summary>
	/// Returns a collection of beatmap objects for the given beatmap IDs.
	/// </summary>
	/// <param name="beatmapIds"></param>
	/// <returns></returns>
	Task<IEnumerable<Beatmap>> GetAsync(IEnumerable<long> beatmapIds);
}