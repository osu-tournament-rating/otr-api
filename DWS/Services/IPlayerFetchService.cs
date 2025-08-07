namespace DWS.Services;

/// <summary>
/// Service for fetching player data from the osu! API and persisting it to the database.
/// </summary>
/// <remarks>
/// This service handles both creating new player records and updating existing player data
/// with the latest information from the osu! API.
/// </remarks>
public interface IPlayerFetchService
{
    /// <summary>
    /// Fetches and stores data for a player
    /// </summary>
    /// <remarks>
    /// This method will create players who do not exist and update existing player data.
    /// </remarks>
    /// <param name="osuPlayerId">The osu! ID of the player to fetch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if succeeded, false otherwise</returns>
    Task<bool> FetchAndPersistAsync(long osuPlayerId, CancellationToken cancellationToken = default);
}
