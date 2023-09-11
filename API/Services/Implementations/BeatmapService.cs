using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class BeatmapService : ServiceBase<Beatmap>, IBeatmapService
{
	private readonly ILogger<BeatmapService> _logger;
	private readonly IMapper _mapper;
	private readonly OtrContext _context;

	public BeatmapService(ILogger<BeatmapService> logger, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_mapper = mapper;
		_context = context;
	}

	public async Task<IEnumerable<BeatmapDTO>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds)
	{
		using (_context)
		{
			return _mapper.Map<IEnumerable<BeatmapDTO>>(await _context.Beatmaps.Where(b => beatmapIds.Contains(b.BeatmapId)).ToListAsync());
		}
	}

	public async Task<IEnumerable<BeatmapDTO>> GetAllAsync()
	{
		using (_context)
		{
			return _mapper.Map<IEnumerable<BeatmapDTO>>(await _context.Beatmaps.ToListAsync());
		}
	}

	public async Task<BeatmapDTO?> GetByBeatmapIdAsync(long osuBeatmapId)
	{
		using (_context)
		{
			return _mapper.Map<BeatmapDTO?>(await _context.Beatmaps.FirstOrDefaultAsync(b => b.BeatmapId == osuBeatmapId));
		}
	}
}