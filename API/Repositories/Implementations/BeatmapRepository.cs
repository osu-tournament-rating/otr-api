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

	public async Task<HashSet<(int, OsuEnums.Mods)>> GetUnprocessedSrBeatmapIdsAsync()
	{
		var mods = new[] { OsuEnums.Mods.HardRock, OsuEnums.Mods.DoubleTime, OsuEnums.Mods.Easy, OsuEnums.Mods.HalfTime };
		var disregardedModsMatrix = new Dictionary<OsuEnums.Mode, OsuEnums.Mods[]>
		{
			{ OsuEnums.Mode.Mania, new[] { OsuEnums.Mods.HardRock, OsuEnums.Mods.Easy } },
			{ OsuEnums.Mode.Taiko, new[] { OsuEnums.Mods.HardRock, OsuEnums.Mods.Easy } }
		};

		var existing = _context.BeatmapModSrs
		                       .Select(b => new
		                       {
			                       b.BeatmapId,
			                       b.Mods
		                       })
		                       .ToImmutableHashSet();

		var all = await _context.Beatmaps.ToDictionaryAsync(b => b.Id, b => b.GameMode);

		HashSet<(int, OsuEnums.Mods)> toProcess = new();

		foreach (var mod in mods)
		{
			foreach ((int id, int mode) in all)
			{
				var modeEnum = (OsuEnums.Mode)mode;

				if (disregardedModsMatrix.ContainsKey(modeEnum) && disregardedModsMatrix[modeEnum].Contains(mod))
				{
					continue;
				}

				var toCheck = new { BeatmapId = id, Mods = (int)mod };
				if (!existing.Contains(toCheck))
				{
					toProcess.Add((id, mod));
				}
			}
		}

		return toProcess;
	}

	public async Task BulkInsertAsync(IEnumerable<BeatmapModSr> beatmapModSrs)
	{
		var modSrs = beatmapModSrs.ToList();
		
		await _context.BeatmapModSrs.AddRangeAsync(modSrs);
		await _context.SaveChangesAsync();
		_logger.LogDebug("Successfully inserted {Count} {T} as part of a bulk insert operation", modSrs.Count, typeof(BeatmapModSr));
	}

	public async Task<double> GetDoubleTimeSrAsync(int beatmapId) => await GetSrForModAsync(beatmapId, OsuEnums.Mods.DoubleTime);
	public async Task<double> GetHardRockSrAsync(int beatmapId) => await GetSrForModAsync(beatmapId, OsuEnums.Mods.HardRock);
	public async Task<double> GetEasySrAsync(int beatmapId) => await GetSrForModAsync(beatmapId, OsuEnums.Mods.Easy);
	public async Task<double> GetHalfTimeSrAsync(int beatmapId) => await GetSrForModAsync(beatmapId, OsuEnums.Mods.HalfTime);

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

	private async Task<double> GetSrForModAsync(int beatmapId, OsuEnums.Mods mod)
	{
		var existingSr = await _context.BeatmapModSrs.FirstOrDefaultAsync(b => b.BeatmapId == beatmapId && b.Mods == (int)mod);
		if (existingSr != null)
		{
			return existingSr.PostModSr;
		}

		_logger.LogWarning("Failed to identify PostModSr for beatmap {BeatmapId} with mod {Mod}! Returning default " +
		                   "value, this may be inaccurate!", beatmapId, mod);

		var existingBeatmap = await _context.Beatmaps.FirstOrDefaultAsync(x => x.Id == beatmapId);
		if (existingBeatmap == null)
		{
			_logger.LogError("Failed to resolve beatmap {BeatmapId} from database while looking for PostModSr! " +
			                 "Returning an unexpected value of -1!", beatmapId);
		}

		return -1;
	}
}