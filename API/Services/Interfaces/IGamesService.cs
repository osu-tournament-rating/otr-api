
using API.Models;

namespace API.Services.Interfaces;

public interface IGamesService : IService<Game>
{
	Task<int> CreateIfNotExistsAsync(Game dbGame);
	Task<Game?> GetByOsuGameIdAsync(long osuGameId);
	Task<IEnumerable<Game>> GetByMatchIdAsync(long matchId);
}