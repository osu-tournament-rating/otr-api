namespace DWS.Services;

public interface IBeatmapFetchService
{
    Task<bool> FetchAndPersistBeatmapAsync(long beatmapId, CancellationToken cancellationToken = default);
}
