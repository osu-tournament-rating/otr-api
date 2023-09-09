using API.Entities;

namespace API.Services.Interfaces;

public interface IGamesService : IService<Game>
{
	Task<IEnumerable<Game>> GetForPlayerAsync(int playerId);
	Task<ulong> BulkInsertAsync(IEnumerable<Game> games);
	Task<IEnumerable<Game>> GetByGameIdsAsync(IEnumerable<int> gameIds);
	Task<Dictionary<long, int>> GetGameIdMappingAsync(IEnumerable<long> beatmapIds);
	Task<int> CreateIfNotExistsAsync(Game dbGame);
	Task<Game?> GetByOsuGameIdAsync(long osuGameId);
	Task<Match?> GetMatchByGameIdAsync(int gameId);
	Task<Game?> GetByGameIdAsync(long gameGameId);

	Task<IEnumerable<Game>> GetByMatchIdAsync(long matchId);
}