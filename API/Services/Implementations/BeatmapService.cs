using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace API.Services.Implementations;

public class BeatmapService : ServiceBase<Beatmap>, IBeatmapService
{
	private readonly OtrContext _context;
	private readonly ILogger<BeatmapService> _logger;
	private readonly IMapper _mapper;

	public BeatmapService(ILogger<BeatmapService> logger, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_mapper = mapper;
		_context = context;
	}

	public async Task<long> GetBeatmapIdAsync(int id) => await _context.Beatmaps.Where(b => b.Id == id).Select(b => b.BeatmapId).FirstOrDefaultAsync();

	public async Task<IEnumerable<BeatmapDTO>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds) =>
		_mapper.Map<IEnumerable<BeatmapDTO>>(await _context.Beatmaps.Where(b => beatmapIds.Contains(b.BeatmapId)).ToListAsync());

	public async Task<IEnumerable<BeatmapDTO>> GetAllAsync() => _mapper.Map<IEnumerable<BeatmapDTO>>(await _context.Beatmaps.ToListAsync());

	public async Task<BeatmapDTO?> GetByBeatmapIdAsync(long osuBeatmapId) =>
		_mapper.Map<BeatmapDTO?>(await _context.Beatmaps.FirstOrDefaultAsync(b => b.BeatmapId == osuBeatmapId));

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
		await _context.BeatmapModSrs.AddRangeAsync(beatmapModSrs);
		await _context.SaveChangesAsync();
		_logger.LogDebug("Successfully inserted {Count} {T} as part of a bulk insert operation", beatmapModSrs.Count(), typeof(BeatmapModSr));
	}

	public async Task InsertModSrAsync(BeatmapModSr beatmapModSr)
	{
		await _context.BeatmapModSrs.AddAsync(beatmapModSr);
		await _context.SaveChangesAsync();
	}

	public async Task<double> GetDoubleTimeSrAsync(int beatmapId) => await GetSrForModAsync(beatmapId, OsuEnums.Mods.DoubleTime);
	public async Task<double> GetHardRockSrAsync(int beatmapId) => await GetSrForModAsync(beatmapId, OsuEnums.Mods.HardRock);
	public async Task<double> GetEasySrAsync(int beatmapId) => await GetSrForModAsync(beatmapId, OsuEnums.Mods.Easy);
	public async Task<double> GetHalfTimeSrAsync(int beatmapId) => await GetSrForModAsync(beatmapId, OsuEnums.Mods.HalfTime);

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