using API.Entities;

namespace API.Services.Interfaces;

public interface IBeatmapService : IService<Beatmap>
{
	Task<IEnumerable<Beatmap>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds);
	/// <summary>
	/// Count of inserted rows
	/// </summary>
	/// <param name="beatmaps"></param>
	Task<int> BulkInsertAsync(IEnumerable<Beatmap> beatmaps);
}