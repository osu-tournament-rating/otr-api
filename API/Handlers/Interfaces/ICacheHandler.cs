using API.DTOs;
using CachingFramework.Redis.Contracts;

namespace API.Handlers.Interfaces;

public interface ICacheHandler : IContext
{
    /// <summary>
    /// Adds a list of tournament search results to the cache, indexed by the given query
    /// </summary>
    Task SetTournamentSearchResultAsync(IEnumerable<TournamentSearchResultDTO> result, string query);

    /// <summary>
    /// Adds a list of match search results to the cache, indexed by the given query
    /// </summary>
    Task SetMatchSearchResultAsync(IEnumerable<MatchSearchResultDTO> result, string query);

    /// <summary>
    /// Adds a list of player search results to the cache, indexed by the given query
    /// </summary>
    Task SetPlayerSearchResultAsync(IEnumerable<PlayerSearchResultDTO> result, string query);

    /// <summary>
    /// Invalidates all cached objects with lifetimes that depend on tournament CRUD actions
    /// </summary>
    Task OnTournamentUpdateAsync();
}
