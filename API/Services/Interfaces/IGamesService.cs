
using API.DTOs;
using API.Models;

namespace API.Services.Interfaces;

public interface IGamesService : IService<Game>
{
	Task<int> CreateIfNotExistsAsync(Game dbGame);
	Task<GameDTO?> GetByOsuGameIdAsync(long osuGameId);
	Task<IEnumerable<GameDTO>> GetByMatchIdAsync(long matchId);
}