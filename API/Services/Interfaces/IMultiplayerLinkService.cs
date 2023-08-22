using API.Entities;

namespace API.Services.Interfaces;

public interface IMultiplayerLinkService : IService<MultiplayerLink>
{
	Task<MultiplayerLink?> GetByLobbyIdAsync(long lobbyId);
	Task<IEnumerable<MultiplayerLink>?> GetAllPendingAsync();
	Task<MultiplayerLink?> GetFirstPendingOrDefaultAsync();
	Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> lobbyIds);
}