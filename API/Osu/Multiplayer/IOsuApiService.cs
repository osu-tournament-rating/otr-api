using API.Entities;

namespace API.Osu.Multiplayer;

public interface IOsuApiService
{
	Task<OsuApiMatchData?> GetMatchAsync(long matchId);
	Task<OsuApiUser?> GetUserAsync(long userId, OsuEnums.Mode mode);
	Task<Beatmap?> GetBeatmapAsync(long beatmapId);
}