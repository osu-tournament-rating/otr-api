using API.Entities;

namespace API.Repositories.Interfaces;

public interface IBeatmapRepository : IRepository<Beatmap>
{
	Task<long> GetBeatmapIdAsync(int id);
	Task<Beatmap?> GetByOsuIdAsync(long osuBeatmapId);
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