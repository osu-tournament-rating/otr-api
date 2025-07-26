namespace DWS.Services;

/// <summary>
/// Service for fetching and persisting player historical statistics data from the osu!track API.
/// </summary>
public interface IPlayerOsuTrackFetchService
{
    /// <summary>
    /// Fetches historical statistics data for a player from the osu!track API and updates their database records.
    /// </summary>
    /// <param name="osuPlayerId">The osu! player ID to fetch data for.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>True if the operation was successful, false otherwise.</returns>
    Task<bool> FetchAndPersistAsync(long osuPlayerId, CancellationToken cancellationToken = default);
}
