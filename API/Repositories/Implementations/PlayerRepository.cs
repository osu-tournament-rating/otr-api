using API.DTOs;
using API.Entities;
using API.Osu;
using API.Repositories.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
{
	private readonly OtrContext _context;
	private readonly IMapper _mapper;

	public PlayerRepository(OtrContext context, IMapper mapper) : base(context)
	{
		_context = context;
		_mapper = mapper;
	}

	public override async Task<int> UpdateAsync(Player player)
	{
		player.Updated = DateTime.UtcNow;
		return await base.UpdateAsync(player);
	}

	public override async Task<Player?> CreateAsync(Player player)
	{
		player.Created = DateTime.UtcNow;
		return await base.CreateAsync(player);
	}

	public async Task<IEnumerable<Player>> GetPlayersMissingRankAsync()
	{
		// Get all players that are missing an earliest global rank in any mode (but have a current rank in that mode)
		var players = await _context.Players.Where(x => (x.EarliestOsuGlobalRank == null && x.RankStandard != null) ||
		                                                (x.EarliestTaikoGlobalRank == null && x.RankTaiko != null) ||
		                                                (x.EarliestCatchGlobalRank == null && x.RankCatch != null) ||
		                                                (x.EarliestManiaGlobalRank == null && x.RankMania != null))
		                            .ToListAsync();

		return players;
	}

	public async Task<IEnumerable<Player>> GetAsync(IEnumerable<long> osuIds) => await _context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync();

	public async Task<IEnumerable<Player>> GetAsync(bool eagerLoad)
	{
		if (eagerLoad)
		{
			return await _context.Players
			                     .Include(x => x.MatchScores)
			                     .Include(x => x.Ratings)
			                     .AsNoTracking()
			                     .ToListAsync();
		}

		return await _context.Players.AsNoTracking().ToListAsync();
	}

	public async Task<Player?> GetAsync(string username) =>
		await _context.Players.Where(p => p.Username != null && p.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();

	public async Task<Player?> GetAsync(long osuId, bool eagerLoad = false, int mode = 0, int offsetDays = -1)
	{
		if (!eagerLoad)
		{
			return await _context.Players.WhereOsuId(osuId).FirstOrDefaultAsync();
		}

		var time = offsetDays == -1 ? DateTime.MinValue : DateTime.UtcNow.AddDays(-offsetDays);

		var p = await _context.Players
		                      .Include(x => x.MatchScores.Where(y => y.Game.StartTime > time && y.Game.PlayMode == mode))
		                      .ThenInclude(x => x.Game)
		                      .ThenInclude(x => x.Match)
		                      .Include(x => x.Ratings.Where(y => y.Mode == mode))
		                      .Include(x => x.User)
		                      .WhereOsuId(osuId)
		                      .FirstOrDefaultAsync();

		if (p == null)
		{
			return null;
		}

		return p;
	}

	public async Task<int?> GetIdAsync(long osuId) => await _context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();
	public async Task<long> GetOsuIdAsync(int id) => await _context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();

	public async Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode) => await (from p in _context.Players
	                                                                                                        join r in _context.BaseStats on p.Id equals r.PlayerId
	                                                                                                        where r.Mode == (int)mode
	                                                                                                        orderby r.Rating descending
	                                                                                                        select new PlayerRatingDTO
	                                                                                                        {
		                                                                                                        OsuId = p.OsuId,
		                                                                                                        Username = p.Username ?? "Unknown User",
		                                                                                                        Mu = r.Rating,
		                                                                                                        Sigma = r.Volatility
	                                                                                                        })
	                                                                                                       .Take(n)
	                                                                                                       .ToListAsync();

	public async Task<string?> GetUsernameAsync(long? osuId) => await _context.Players.WhereOsuId(osuId).Select(p => p.Username).FirstOrDefaultAsync();
	public async Task<string?> GetUsernameAsync(int? id) => await _context.Players.Where(p => p.Id == id).Select(p => p.Username).FirstOrDefaultAsync();
	public async Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync() => (await _context.Players.AsNoTracking()
		.ToDictionaryAsync(p => p.OsuId, p => p.Id))
		.OrderBy(x => x.Value)
		.Select(x => new PlayerIdMappingDTO
		{
			Id = x.Value,
			OsuId = x.Key
		});

	public async Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync() => await _context.Players
		.AsNoTracking()
		.OrderBy(x => x.Id)
		.Select(x => new PlayerCountryMappingDTO
		{
			PlayerId = x.Id,
			Country = x.Country
		}).ToListAsync();

	public async Task<int> GetIdByUserIdAsync(int userId) => await _context.Players.AsNoTracking()
	                                                                       .Where(x => x.User != null && x.User.Id == userId)
	                                                                       .Select(x => x.Id)
	                                                                       .FirstOrDefaultAsync();

	public async Task<string?> GetCountryAsync(int playerId) => await _context.Players.Where(p => p.Id == playerId).Select(p => p.Country).FirstOrDefaultAsync();

	public async Task<int> GetIdAsync(string username)
	{
		if (username.Contains(' '))
		{
			// Look for users with either ' ' or '_' in the name - osu only uses one (i.e. "Red Pixel" cannot coexist with "Red_Pixel")
			return await _context.Players.Where(p => p.Username != null && (p.Username.ToLower() == username.ToLower() || p.Username.ToLower() == username.Replace(' ', '_')))
			                     .Select(p => p.Id)
			                     .FirstOrDefaultAsync();
		}

		return await _context.Players.Where(p => p.Username != null && p.Username.ToLower() == username.ToLower()).Select(p => p.Id).FirstOrDefaultAsync();
	}

	// This is used by a scheduled task to automatically populate user info, such as username, country, etc.
	public async Task<IEnumerable<Player>> GetOutdatedAsync() =>
		await _context.Players.Where(p => p.Updated == null || (DateTime.UtcNow - p.Updated) > TimeSpan.FromDays(14)).ToListAsync();

	public async Task<PlayerInfoDTO?> GetPlayerDTOByOsuIdAsync(long osuId, bool eagerLoad = false, OsuEnums.Mode mode = OsuEnums.Mode.Standard, int offsetDays = -1)
	{
		var obj = _mapper.Map<PlayerInfoDTO?>(await GetAsync(osuId, eagerLoad, (int)mode, offsetDays));

		if (obj == null)
		{
			return obj;
		}

		return obj;
	}
}