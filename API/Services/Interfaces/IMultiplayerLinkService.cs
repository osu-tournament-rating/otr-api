using API.Entities;

namespace API.Services.Interfaces;

public interface IMultiplayerLinkService : IService<Match>
{
	Task<Match?> GetByLobbyIdAsync(long matchId);
	Task<IEnumerable<Match>?> GetAllPendingVerificationAsync();
	Task<Match?> GetFirstPendingOrDefaultAsync();
	Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds);
}