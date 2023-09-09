using API.Configurations;
using API.Models;
using API.Services.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace API.Services.Implementations;

public class PlayerService : ServiceBase<Player>, IPlayerService
{
	private readonly OtrContext _context;

	public PlayerService(ILogger<PlayerService> logger, OtrContext context) : base(logger) { _context = context; }

	public async Task<IEnumerable<Player>> GetAllAsync()
	{
		using(var context = new OtrContext())
		{
			return await context.Players.ToListAsync();
		}
	}

	public async Task<Player?> GetByOsuIdAsync(long osuId)
	{
		using (_context)
		{
			return await _context.Players
			                     .Include(x => x.MatchScores)
			                     .Include(x => x.RatingHistories)
			                     .Include(x => x.Ratings)
			                     .Include(x => x.User)
			                     .FirstOrDefaultAsync();
		}
	}

	public async Task<IEnumerable<Player>> GetByOsuIdsAsync(IEnumerable<long> osuIds)
	{
		using (_context)
		{
			return await _context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync();
		}
	}

	public async Task<int> GetIdByOsuIdAsync(long osuId)
	{
		using (_context)
		{
			return await _context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();
		}
	}

	public async Task<long> GetOsuIdByIdAsync(int id)
	{
		using (_context)
		{
			return await _context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();
		}
	}

	public async Task<IEnumerable<Player>> GetOutdatedAsync()
	{
		using (_context)
		{
			return await _context.Players.Where(p => p.Updated < DateTime.Now.Subtract(TimeSpan.FromDays(14)) || p.Updated == null).ToListAsync();
		}
	}
}