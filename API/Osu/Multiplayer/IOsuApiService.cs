namespace API.Osu.Multiplayer;

public interface IOsuApiService
{
	Task<OsuApiMatchData?> GetMatchAsync(long matchId);
}