using API.Configurations;
using API.Models;
using API.Services.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace API.Services.Implementations;

public class PlayerService : ServiceBase<Player>, IPlayerService
{
	private readonly IServiceProvider _serviceProvider;
	public PlayerService(ILogger<PlayerService> logger, IServiceProvider serviceProvider) : base(logger)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task<IEnumerable<Player>> GetAllAsync()
	{
		using(var context = new OtrContext())
		{
			return await context.Players.ToListAsync();
		}
	}

	public async Task<Player?> GetByOsuIdAsync(long osuId)
	{
		using (var context = new OtrContext())
		{
			return await context.Players.FirstOrDefaultAsync(p => p.OsuId == osuId);
		}
	}

	public async Task<IEnumerable<Player>> GetByOsuIdsAsync(IEnumerable<long> osuIds)
	{
		using (var context = new OtrContext())
		{
			return await context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync();
		}
	}

	public async Task<int> GetIdByOsuIdAsync(long osuId)
	{
		using (var context = new OtrContext())
		{
			return await context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();
		}
	}

	public async Task<long> GetOsuIdByIdAsync(int id)
	{
		using (var context = new OtrContext())
		{
			return await context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();
		}
	}

	public async Task<IEnumerable<Player>> GetOutdatedAsync()
	{
		using (var context = new OtrContext())
		{
			return await context.Players.Where(p => p.Updated < DateTime.Now.Subtract(TimeSpan.FromDays(14)) || p.Updated == null).ToListAsync();
		}
	}
}