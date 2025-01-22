using Database.Entities;
using OsuApiClient.Domain.Osu.Beatmaps;
using OsuApiClient.Domain.Osu.Multiplayer;
using Beatmap = Database.Entities.Beatmap;

namespace DataWorkerService.Services.Interfaces;

/// <summary>
/// Interfaces the <see cref="MatchApiDataParserService"/>
/// </summary>
public interface IOsuApiDataParserService
{
    /// <summary>
    /// Parses data from a <see cref="MultiplayerMatch"/> into a <see cref="Match"/>
    /// </summary>
    /// <remarks>
    /// Creates (or updates existing) <see cref="Game"/>s, <see cref="Player"/>s, <see cref="Database.Entities.Beatmap"/>s,
    /// and <see cref="Database.Entities.GameScore"/>s.
    /// Does not save changes to the database.
    /// </remarks>
    /// <param name="match"><see cref="Match"/> to populate with data</param>
    /// <param name="apiMatch"><see cref="MultiplayerMatch"/> data</param>
    Task ParseMatchAsync(Match match, MultiplayerMatch apiMatch);

    Task ParseBeatmapAsync(Beatmap beatmap, BeatmapExtended apiBeatmap);
}
