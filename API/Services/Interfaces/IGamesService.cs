
using API.Models;

namespace API.Services.Interfaces;

public interface IGamesService : IService<Game>
{
	Task<IEnumerable<Game>> GetForPlayerAsync(int playerId);
	Task<IEnumerable<Game>> GetByGameIdsAsync(IEnumerable<int> gameIds);
	Task<Dictionary<long, int>> GetGameIdMappingAsync(IEnumerable<long> beatmapIds);
	Task<int> CreateIfNotExistsAsync(Game dbGame);
	Task<Game?> GetByOsuGameIdAsync(long osuGameId);
	Task<IEnumerable<Game>> GetByMatchIdAsync(long matchId);
}