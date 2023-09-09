using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class BeatmapService : ServiceBase<Beatmap>, IBeatmapService
{
	private readonly ILogger<BeatmapService> _logger;
	public BeatmapService(ILogger<BeatmapService> logger) : base(logger) { _logger = logger; }

	public async Task<IEnumerable<Beatmap>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds)
	{
		using (var context = new OtrContext())
		{
			return await context.Beatmaps.Where(b => beatmapIds.Contains(b.BeatmapId)).ToListAsync();
		}
	}

	public async Task<IEnumerable<Beatmap>> GetAllAsync()
	{
		using (var context = new OtrContext())
		{
			return await context.Beatmaps.ToListAsync();
		}
	}

	public async Task<Beatmap?> GetByBeatmapIdAsync(long osuBeatmapId)
	{
		using (var context = new OtrContext())
		{
			return await context.Beatmaps.FirstOrDefaultAsync(b => b.BeatmapId == osuBeatmapId);
		}
	}
}