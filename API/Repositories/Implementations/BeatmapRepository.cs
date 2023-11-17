using API.Entities;
using API.Osu;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace API.Repositories.Implementations;

public class BeatmapRepository : RepositoryBase<Beatmap>, IBeatmapRepository
{
	private readonly OtrContext _context;
	private readonly ILogger<BeatmapRepository> _logger;

	public BeatmapRepository(ILogger<BeatmapRepository> logger, OtrContext context) : base(context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<long> GetBeatmapIdAsync(int id) => await _context.Beatmaps.Where(b => b.Id == id).Select(b => b.BeatmapId).FirstOrDefaultAsync();
	public async Task<Beatmap?> GetByOsuIdAsync(long osuBeatmapId) => await _context.Beatmaps.FirstOrDefaultAsync(x => x.BeatmapId == osuBeatmapId);

	public async Task<Beatmap?> GetBeatmapByBeatmapIdAsync(long osuBeatmapId)
	{
		return await _context.Beatmaps.Where(x => x.BeatmapId == osuBeatmapId).FirstOrDefaultAsync();
	}

	public async Task CreateIfNotExistsAsync(IEnumerable<long> beatmapIds)
	{
		foreach (long id in beatmapIds)
		{
			if (!await _context.Beatmaps.AnyAsync(x => x.BeatmapId == id))
			{
				await _context.Beatmaps.AddAsync(new Beatmap
				{
					BeatmapId = id
				});
			}
		}

		await _context.SaveChangesAsync();
	}

	public async Task<int?> GetIdByBeatmapIdAsync(long gameBeatmapId)
	{
		if (!await _context.Beatmaps.AnyAsync(x => x.BeatmapId == gameBeatmapId))
		{
			return null;
		}
		
		return await _context.Beatmaps.Where(x => x.BeatmapId == gameBeatmapId).Select(x => x.Id).FirstOrDefaultAsync();
	}

	public async Task<IEnumerable<Beatmap>> GetAllAsync() => await _context.Beatmaps.ToListAsync();
	public async Task<IEnumerable<Beatmap>> GetAsync(IEnumerable<long> beatmapIds) => await _context.Beatmaps.Where(b => beatmapIds.Contains(b.BeatmapId)).ToListAsync();
}