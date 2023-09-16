
using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IGamesService : IService<Game>
{
	Task<int> CreateIfNotExistsAsync(Game dbGame);
	Task<GameDTO?> GetByOsuGameIdAsync(long osuGameId);
	Task<IEnumerable<GameDTO>> GetByMatchIdAsync(long matchId);
	Task<IEnumerable<Game>> GetAllAsync();
	Task UpdateAllPostModSrsAsync();
	Task<int> CountGameWinsAsync(long osuPlayerId, int mode, DateTime fromTime);
}