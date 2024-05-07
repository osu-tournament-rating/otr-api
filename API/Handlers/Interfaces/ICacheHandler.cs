using API.DTOs;
using CachingFramework.Redis.Contracts;

namespace API.Handlers.Interfaces;

public interface ICacheHandler : IContext
{
    void SetTournamentSearchResult(IEnumerable<TournamentSearchResultDTO> result, string query);

    void SetMatchSearchResult(IEnumerable<MatchSearchResultDTO> result, string query);

    void SetPlayerSearchResult(IEnumerable<PlayerSearchResultDTO> result, string query);
}
