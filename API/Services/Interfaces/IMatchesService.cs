using API.DTOs;
using Database.Enums;

namespace API.Services.Interfaces;

public interface IMatchesService
{
    /// <summary>
    /// Gets a paged list of matches
    /// </summary>
    /// <remarks>Matches are ordered by primary key</remarks>
    /// <param name="limit">Amount of matches to return. Functions as the "page size"</param>
    /// <param name="page">Which block of matches to return</param>
    Task<PagedResultDTO<MatchDTO>> GetAsync(
        int limit,
        int page,
        MatchesFilterDTO filter
    );

    /// <summary>
    /// Gets a match for the given id including all navigation properties.
    /// </summary>
    /// <param name="id">Match id</param>
    Task<MatchDTO?> GetAsync(int id);

    Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        Ruleset ruleset,
        DateTime start,
        DateTime end
    );

    /// <summary>
    /// Searches for the specified match by name
    /// </summary>
    /// <param name="name">The partial search key for the match name</param>
    /// <returns>Up to 30 results if any matches are found</returns>
    Task<IEnumerable<MatchSearchResultDTO>> SearchAsync(string name);
}
