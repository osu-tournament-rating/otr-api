namespace DWS.Services;

/// <summary>
/// Service for fetching beatmap data from the osu! API and persisting them to the database.
/// </summary>
public interface IBeatmapFetchService
{
    /// <summary>
    /// Fetches beatmap data by ID and persists it to the database.
    /// </summary>
    /// <param name="osuBeatmapId">The osu! beatmap ID.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the beatmap was successfully fetched and persisted; otherwise, false.</returns>
    Task<bool> FetchAndPersistBeatmapAsync(long osuBeatmapId, CancellationToken cancellationToken = default);
}
