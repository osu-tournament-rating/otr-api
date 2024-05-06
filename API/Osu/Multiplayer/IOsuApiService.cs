using API.Entities;
using API.Osu.Enums;

namespace API.Osu.Multiplayer;

public interface IOsuApiService
{
    Task<OsuApiMatchData?> GetMatchAsync(long matchId, string reason);
    Task<OsuApiUser?> GetUserAsync(long userId, Ruleset ruleset, string reason);
    Task<Beatmap?> GetBeatmapAsync(long beatmapId, string reason);
}
