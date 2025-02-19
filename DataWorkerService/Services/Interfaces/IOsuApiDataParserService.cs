using Database.Entities;
using OsuApiClient.Domain.Osu.Multiplayer;

namespace DataWorkerService.Services.Interfaces;

/// <summary>
/// Interfaces the <see cref="MatchApiDataParserService"/>
/// </summary>
public interface IOsuApiDataParserService
{
    /// <summary>
    /// Parses the contents of a <see cref="MultiplayerMatch"/> into a <see cref="Match"/>
    /// </summary>
    /// <param name="match">Database match</param>
    /// <param name="apiMatch">osu! API match</param>
    Task ParseMatchAsync(Match match, MultiplayerMatch apiMatch);

    /// <summary>
    /// Gathers and parses all required information for <see cref="Beatmap"/>s and <see cref="BeatmapSet"/>s for a
    /// list of osu! ids
    /// </summary>
    /// <param name="beatmapOsuIds">Beatmap osu! ids</param>
    Task ProcessBeatmapsAsync(IEnumerable<long> beatmapOsuIds);
}
