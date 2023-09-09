using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class GamesService : ServiceBase<Game>, IGamesService
{
	public GamesService(ILogger<GamesService> logger) : base(logger)
	{
	}

	public async Task<IEnumerable<Game>> GetForPlayerAsync(int playerId)
	{
		using (var context = new OtrContext())
		{
			return await context.Games.Where(g => g.MatchScores.Any(x => x.PlayerId == playerId)).ToListAsync();
		}
	}

	public async Task<IEnumerable<Game>> GetByGameIdsAsync(IEnumerable<int> gameIds)
	{
		using (var context = new OtrContext())
		{
			return await context.Games
			                    .Where(game => gameIds.Contains(game.Id))
			                    .ToListAsync();
		}
	}

	public async Task<int> CreateIfNotExistsAsync(Game dbGame)
	{
		using (var context = new OtrContext())
		{
			var existingGame = await context.Games.FirstOrDefaultAsync(g => g.MatchId == dbGame.MatchId && g.BeatmapId == dbGame.BeatmapId);
			if (existingGame != null)
			{
				return existingGame.Id;
			}

			await context.Games.AddAsync(dbGame);
			await context.SaveChangesAsync();
			return dbGame.Id;
		}
	}

	public async Task<Game?> GetByOsuGameIdAsync(long osuGameId)
	{
		using (var context = new OtrContext())
		{
			return await context.Games.FirstOrDefaultAsync(g => g.GameId == osuGameId);
		}
	}

	public async Task<IEnumerable<Game>> GetByMatchIdAsync(long matchId)
	{
		using (var context = new OtrContext())
		{
			int id = await context.Matches.Where(match => match.MatchId == matchId).Select(match => match.Id).FirstOrDefaultAsync();
			return await context.Games.Where(game => game.MatchId == id).ToListAsync();
		}
	}
}