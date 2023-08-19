using API.Entities;
using API.Osu.Multiplayer;

namespace API.Services.Interfaces;

public interface IMultiplayerLinkService : IService<MultiplayerLink>
{
	Task<MultiplayerLink?> GetByLobbyIdAsync(long lobbyId);
	Task<IEnumerable<MultiplayerLink>?> GetAllPendingAsync();
	Task<MultiplayerLink?> GetFirstPendingOrDefaultAsync();
}