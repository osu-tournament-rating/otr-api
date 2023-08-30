using API.Entities;

namespace API.Services.Interfaces;

public interface IMultiplayerLinkService : IService<OsuMatch>
{
	Task<OsuMatch?> GetByLobbyIdAsync(long matchId);
	Task<IEnumerable<OsuMatch>?> GetAllPendingVerificationAsync();
	Task<OsuMatch?> GetFirstPendingOrDefaultAsync();
	Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds);
}