using API.DTOs;

namespace API.Services.Interfaces;

public interface ISearchService
{
    /// <summary>
    /// Takes in a string query and lookups up matched tournaments, matches, or usernames. This search performs a partial match search.
    /// </summary>
    /// <param name="tournamentName"></param>
    /// <param name="matchName"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<List<SearchResponseDTO>> SearchByNameAsync(string? tournamentName, string? matchName, string? username);
}
