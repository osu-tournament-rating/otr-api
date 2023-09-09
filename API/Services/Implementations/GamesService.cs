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

	public async Task<Dictionary<long, int>> GetGameIdMappingAsync(IEnumerable<long> beatmapIds)
	{
		HashSet<long> beatmapIdsHashSet = beatmapIds.ToHashSet();
		Dictionary<long, int> beatmapIdMapping;
		
		using (var context = new OtrContext())
		{
			beatmapIdMapping = await context.Beatmaps
			                               .Where(b => beatmapIdsHashSet.Contains(b.BeatmapId))
			                               .ToDictionaryAsync(b => b.BeatmapId, b => b.Id);
		}

		using (var context = new OtrContext())
		{
			return await context.Games
			                    .Where(g => g.BeatmapId != null && beatmapIdMapping.Values.Contains(g.BeatmapId))
			                    .ToDictionaryAsync(g => g.BeatmapId, g => g.Id);
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
	public async Task<Game?> GetByOsuGameIdAsync(long osuGameId) => throw new NotImplementedException();
	public async Task<IEnumerable<Game>> GetByMatchIdAsync(long matchId) => throw new NotImplementedException();
}