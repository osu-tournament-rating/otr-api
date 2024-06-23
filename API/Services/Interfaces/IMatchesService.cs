using API.DTOs;
using Database.Enums.Verification;

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
    /// <remarks>Matches are ordered by primary key</remarks>
    /// <param name="limit">Amount of matches to return. Functions as the "page size"</param>
    /// <param name="page">Which block of matches to return</param>
    /// <param name="filterUnverified">If unverified matches should be excluded from the results</param>
    Task<PagedResultDTO<MatchDTO>> GetAsync(int limit, int page, bool filterUnverified = true);

    Task<MatchDTO?> GetByOsuIdAsync(long osuMatchId);

    Task<MatchDTO?> GetAsync(int id, bool filterInvalid = true);

    /// <summary>
    /// Updates the verification status of a match for the given id
    /// </summary>
    /// <param name="id">Id of the match</param>
    /// <param name="verificationStatus">New verification status to assign</param>
    /// <param name="verifierId">Optional user id to attribute the update to</param>
    /// <returns>An updated match, or null if not found</returns>
    Task<MatchDTO?> UpdateVerificationStatusAsync(
        int id,
        VerificationStatus verificationStatus,
        int? verifierId = null
    );

    Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        int mode,
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
