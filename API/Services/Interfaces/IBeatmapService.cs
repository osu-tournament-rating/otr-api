using API.DTOs;

namespace API.Services.Interfaces;

public interface IBeatmapService
{
	Task<IEnumerable<BeatmapDTO>> GetAllAsync();
	Task<IEnumerable<BeatmapDTO>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds);
	Task<BeatmapDTO?> GetAsync(long osuBeatmapId);
}