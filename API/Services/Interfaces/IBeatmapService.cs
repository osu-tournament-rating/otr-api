using API.DTOs;
using API.Models;

namespace API.Services.Interfaces;

public interface IBeatmapService : IService<Beatmap>
{
	Task<IEnumerable<BeatmapDTO>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds);
	/// <summary>
	/// Count of inserted rows
	/// </summary>
	/// <param name="beatmaps"></param>
	Task<int> BulkInsertAsync(IEnumerable<Beatmap> beatmaps);

	Task<IEnumerable<BeatmapDTO>> GetAllAsync();
	Task<BeatmapDTO?> GetByBeatmapIdAsync(long osuBeatmapId);
}