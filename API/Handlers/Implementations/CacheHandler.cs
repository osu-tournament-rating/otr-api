using API.DTOs;
using API.Handlers.Interfaces;
using API.Utilities;
using CachingFramework.Redis;

namespace API.Handlers.Implementations;

public class CacheHandler(string configuration) : RedisContext(configuration), ICacheHandler
{
    private const int SearchResultTimeMins = 30;

    public void SetTournamentSearchResult(IEnumerable<TournamentSearchResultDTO> result, string query) =>
        Cache.SetObject(
            CacheUtils.TournamentSearchKey(query),
            result,
            [CacheUtils.TournamentSearchTag],
            TimeSpan.FromMinutes(SearchResultTimeMins)
        );

    public void SetMatchSearchResult(IEnumerable<MatchSearchResultDTO> result, string query) =>
        Cache.SetObject(
            CacheUtils.MatchSearchKey(query),
            result,
            [CacheUtils.MatchSearchTag],
            TimeSpan.FromMinutes(SearchResultTimeMins)
        );

    public void SetPlayerSearchResult(IEnumerable<PlayerSearchResultDTO> result, string query) =>
        Cache.SetObject(
            CacheUtils.PlayerSearchKey(query),
            result,
            [CacheUtils.PlayerSearchTag],
            TimeSpan.FromMinutes(SearchResultTimeMins)
        );
}
