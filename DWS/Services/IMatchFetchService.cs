namespace DWS.Services;

public interface IMatchFetchService
{
    /// <summary>
    /// Fetches a match from the osu! API and persists all related data (games, scores, players).
    /// Always fetches fresh data regardless of whether the match already exists in the database.
    /// </summary>
    /// <param name="osuMatchId">The osu! match ID to fetch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the match was successfully fetched and persisted, false if the match was not found in the osu! API</returns>
    Task<bool> FetchAndPersistMatchAsync(long osuMatchId, CancellationToken cancellationToken = default);
}
