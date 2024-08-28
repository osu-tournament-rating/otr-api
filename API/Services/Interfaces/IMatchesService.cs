using API.DTOs;
using Database.Enums;

namespace API.Services.Interfaces;

public interface IMatchesService
{
    /// <summary>
    /// Creates matches
    /// </summary>
    /// <param name="tournamentId">Id of the parent tournament</param>
    /// <param name="submitterId">Id of the submitting user</param>
    /// <param name="matchIds">List of match ids</param>
    /// <param name="verify">Submitter is a match verifier</param>
    /// <returns>Location information for the created matches, or null if parent tournament does not exist</returns>
    Task<IEnumerable<MatchCreatedResultDTO>?> CreateAsync(
        int tournamentId,
        int submitterId,
        IEnumerable<long> matchIds,
        bool verify
    );

    /// <summary>
    /// Gets a paged list of matches
    /// </summary>
    /// <param name="limit">Amount of matches to return. Functions as the "page size"</param>
    /// <param name="page">Which block of matches to return</param>
    /// <param name="filter">Filtering options</param>
    Task<PagedResultDTO<MatchDTO>> GetAsync(
        int limit,
        int page,
        MatchesFilterDTO filter
    );

    Task<MatchDTO?> GetAsync(
        int id,
        QueryFilterType filterType = QueryFilterType.Verified | QueryFilterType.ProcessingCompleted
    );

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
