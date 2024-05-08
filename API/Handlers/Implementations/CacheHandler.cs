using API.DTOs;
using API.Handlers.Interfaces;
using API.Utilities;
using CachingFramework.Redis;

namespace API.Handlers.Implementations;

public class CacheHandler(string configuration) : RedisContext(configuration), ICacheHandler
{
    private const int SearchResultTimeMins = 30;

    public async Task SetTournamentSearchResultAsync(IEnumerable<TournamentSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.TournamentSearchKey(query),
            result,
            [CacheUtils.TournamentSearchTag],
            TimeSpan.FromMinutes(SearchResultTimeMins)
        );

    public async Task SetMatchSearchResultAsync(IEnumerable<MatchSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.MatchSearchKey(query),
            result,
            [CacheUtils.MatchSearchTag],
            TimeSpan.FromMinutes(SearchResultTimeMins)
        );

    public async Task SetPlayerSearchResultAsync(IEnumerable<PlayerSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.PlayerSearchKey(query),
            result,
            [CacheUtils.PlayerSearchTag],
            TimeSpan.FromMinutes(SearchResultTimeMins)
        );
}
