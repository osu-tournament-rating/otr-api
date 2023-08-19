namespace API.Osu.Multiplayer;

public interface IOsuApiService
{
	Task<MultiplayerLobbyData?> GetMatchAsync(long matchId);
}