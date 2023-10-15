using API.Entities;

namespace API.Repositories.Interfaces;

public interface IGamesRepository : IRepository<Game>
{
	Task<int> CountGameWinsAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<string?> MostPlayedTeammateNameAsync(long osuPlayerId, int mode, DateTime fromDate);
	Task<string?> MostPlayedOpponentNameAsync(long osuPlayerId, int mode, DateTime fromDate);
}