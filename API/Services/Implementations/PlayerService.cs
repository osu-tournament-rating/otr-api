using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class PlayerService : ServiceBase<Player>, IPlayerService
{
	private readonly OtrContext _context;
	private readonly IMapper _mapper;

	public PlayerService(ILogger<PlayerService> logger, OtrContext context, IMapper mapper) : base(logger, context)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<IEnumerable<PlayerDTO>> GetAllAsync() => _mapper.Map<IEnumerable<PlayerDTO>>(await _context.Players
	                                                                                                             .Include(x => x.MatchScores)
	                                                                                                             .Include(x => x.RatingHistories)
	                                                                                                             .Include(x => x.Ratings)
	                                                                                                             .ToListAsync());

	public async Task<Player?> GetPlayerByOsuIdAsync(long osuId, bool eagerLoad = false)
	{
		if (!eagerLoad)
		{
			return await _context.Players.Where(x => x.OsuId == osuId).FirstOrDefaultAsync();
		}

		return await _context.Players
		                     .Include(x => x.MatchScores)
		                     .Include(x => x.RatingHistories)
		                     .Include(x => x.Ratings)
		                     .Include(x => x.User)
		                     .Where(x => x.OsuId == osuId)
		                     .FirstOrDefaultAsync();
	}

	public async Task<PlayerDTO?> GetPlayerDTOByOsuIdAsync(long osuId, bool eagerLoad = false)
	{
		return _mapper.Map<PlayerDTO?>(await GetPlayerByOsuIdAsync(osuId, eagerLoad));
	}

	public async Task<IEnumerable<PlayerDTO>> GetByOsuIdsAsync(IEnumerable<long> osuIds) =>
		_mapper.Map<IEnumerable<PlayerDTO>>(await _context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync());

	public async Task<int> GetIdByOsuIdAsync(long osuId) => await _context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();
	public async Task<long> GetOsuIdByIdAsync(int id) => await _context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();
	public async Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync() => _mapper.Map<IEnumerable<PlayerRanksDTO>>(await _context.Players.ToListAsync());

	public async Task<IEnumerable<Unmapped_PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode) => await (from p in _context.Players
	                                                                                                                 join r in _context.Ratings on p.Id equals r.PlayerId
	                                                                                                                 where r.Mode == (int)mode
	                                                                                                                 orderby r.Mu descending
	                                                                                                                 select new Unmapped_PlayerRatingDTO
	                                                                                                                 {
		                                                                                                                 OsuId = p.OsuId,
		                                                                                                                 Username = p.Username,
		                                                                                                                 Mu = r.Mu,
		                                                                                                                 Sigma = r.Sigma
	                                                                                                                 })
	                                                                                                                .Take(n)
	                                                                                                                .ToListAsync();

	public async Task<IEnumerable<Player>> GetOutdatedAsync() => await _context.Players.Where(p => p.Updated == null).ToListAsync();
}