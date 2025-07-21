namespace DWS.Services;

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
