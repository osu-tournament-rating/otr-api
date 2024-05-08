using API.DTOs;
using CachingFramework.Redis.Contracts;

namespace API.Handlers.Interfaces;

public interface ICacheHandler : IContext
{
    /// <summary>
    /// Adds a list of tournament search results to the cache, indexed by the given query
    /// </summary>
    void SetTournamentSearchResult(IEnumerable<TournamentSearchResultDTO> result, string query);

    /// <summary>
    /// Adds a list of match search results to the cache, indexed by the given query
    /// </summary>
    void SetMatchSearchResult(IEnumerable<MatchSearchResultDTO> result, string query);

    /// <summary>
    /// Adds a list of player search results to the cache, indexed by the given query
    /// </summary>
    void SetPlayerSearchResult(IEnumerable<PlayerSearchResultDTO> result, string query);
}
