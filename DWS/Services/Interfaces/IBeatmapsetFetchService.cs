namespace DWS.Services.Interfaces;

/// <summary>
/// Service for fetching beatmapset data from the osu! API and persisting them to the database.
/// </summary>
public interface IBeatmapsetFetchService
{
    /// <summary>
    /// Fetches beatmapset data by beatmap ID and persists the entire beatmapset to the database.
    /// </summary>
    /// <param name="osuBeatmapId">The osu! beatmap ID to fetch the beatmapset for (NOT the beatmapset ID).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the beatmapset was successfully fetched and persisted; otherwise, false.</returns>
    Task<bool> FetchAndPersistBeatmapsetAsync(long osuBeatmapId, CancellationToken cancellationToken = default);
}
