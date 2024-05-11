using API.DTOs;
using API.Handlers.Interfaces;
using API.Utilities;
using CachingFramework.Redis;

namespace API.Handlers.Implementations;

public class CacheHandler(string configuration) : RedisContext(configuration), ICacheHandler
{
    private const int SearchResultLifetimeMinutes = 30;

    public async Task SetTournamentSearchResultAsync(IEnumerable<TournamentSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.TournamentSearchKey(query),
            result,
            [CacheUtils.TournamentSearchTag],
            TimeSpan.FromMinutes(SearchResultLifetimeMinutes)
        );

    public async Task SetMatchSearchResultAsync(IEnumerable<MatchSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.MatchSearchKey(query),
            result,
            [CacheUtils.MatchSearchTag],
            TimeSpan.FromMinutes(SearchResultLifetimeMinutes)
        );

    public async Task SetPlayerSearchResultAsync(IEnumerable<PlayerSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.PlayerSearchKey(query),
            result,
            [CacheUtils.PlayerSearchTag],
            TimeSpan.FromMinutes(SearchResultLifetimeMinutes)
        );

    public async Task OnTournamentUpdateAsync()
    {
        // Since search uses partial matching on names, all search results need to be invalidated when
        // a new tournament is created, updated, or deleted to ensure search results stay current
        await InvalidateTournamentSearchResultsAsync();
    }

    public async Task OnMatchUpdateAsync()
    {
        await InvalidateMatchSearchResultsAsync();
    }

    public async Task OnPlayerUpdateAsync()
    {
        await InvalidatePlayerSearchResultsAsync();
    }

    public async Task OnBaseStatsUpdateAsync()
    {
        await InvalidatePlayerSearchResultsAsync();
    }

    /// <summary>
    /// Invalidates all tournament search results
    /// </summary>
    private async Task InvalidateTournamentSearchResultsAsync() =>
        await Cache.InvalidateKeysByTagAsync([CacheUtils.TournamentSearchTag]);

    /// <summary>
    /// Invalidates all match search results
    /// </summary>
    private async Task InvalidateMatchSearchResultsAsync() =>
        await Cache.InvalidateKeysByTagAsync([CacheUtils.MatchSearchTag]);

    /// <summary>
    /// Invalidates all player search results
    /// </summary>
    private async Task InvalidatePlayerSearchResultsAsync() =>
        await Cache.InvalidateKeysByTagAsync([CacheUtils.PlayerSearchTag]);
}
