using API.Entities;

namespace API.Osu.Multiplayer;

public interface IOsuApiService
{
	Task<OsuApiMatchData?> GetMatchAsync(long matchId, string reason);
	Task<OsuApiUser?> GetUserAsync(long userId, OsuEnums.Mode mode, string reason);
	Task<Beatmap?> GetBeatmapAsync(long beatmapId, string reason);
}