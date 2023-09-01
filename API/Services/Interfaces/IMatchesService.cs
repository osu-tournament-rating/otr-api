using API.Entities;

namespace API.Services.Interfaces;

public interface IMatchesService : IService<Match>
{
	Task<Match?> GetByLobbyIdAsync(long matchId);
	Task<IEnumerable<Match>?> GetAllPendingVerificationAsync();
	Task<Match?> GetFirstPendingOrDefaultAsync();
	Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds);
	Task<int> InsertFromIdBatchAsync(IEnumerable<Match> matches);
}