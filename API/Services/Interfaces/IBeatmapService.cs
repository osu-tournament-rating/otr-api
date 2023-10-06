using API.DTOs;
using API.Entities;
using API.Osu;

namespace API.Services.Interfaces;

public interface IBeatmapService : IService<Beatmap>
{
	Task<long> GetBeatmapIdAsync(int id);
	Task<IEnumerable<BeatmapDTO>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds);
	Task<IEnumerable<BeatmapDTO>> GetAllAsync();
	Task<Beatmap?> GetBeatmapByBeatmapIdAsync(long osuBeatmapId);
	Task<BeatmapDTO?> GetBeatmapDTOByBeatmapIdAsync(long osuBeatmapId);

	/// <summary>
	///  Returns all beatmap IDs that have not been processed yet with respect to the SR calculation for different mods.
	/// </summary>
	/// <returns>A list of beatmapids that need processing, with the corresponding mod to process</returns>
	Task<HashSet<(int, OsuEnums.Mods)>> GetUnprocessedSrBeatmapIdsAsync();

	Task BulkInsertAsync(IEnumerable<BeatmapModSr> beatmapModSrs);
	Task InsertModSrAsync(BeatmapModSr beatmapModSr);
	Task<double> GetDoubleTimeSrAsync(int beatmapId);
	Task<double> GetHardRockSrAsync(int beatmapId);
	Task<double> GetEasySrAsync(int beatmapId);
	Task<double> GetHalfTimeSrAsync(int beatmapId);
	Task CreateIfNotExistsAsync(IEnumerable<long> beatmapIds);
	Task<int?> GetIdByBeatmapIdAsync(long gameBeatmapId);
}