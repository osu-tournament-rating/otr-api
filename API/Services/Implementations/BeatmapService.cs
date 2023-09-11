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
		var existing = _context.BeatmapModSrs.Select(b => new
		                       {
			                       b.BeatmapId,
			                       b.Mods
		                       })
		                       .ToImmutableHashSet();

		var all = await _context.Beatmaps.Select(b => b.Id).ToListAsync();

		HashSet<(int, OsuEnums.Mods)> toProcess = new();

		foreach (var mod in mods)
		{
			foreach (int id in all)
			{
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
	}

	public async Task InsertModSrAsync(BeatmapModSr beatmapModSr) => await _context.BeatmapModSrs.AddAsync(beatmapModSr);
}