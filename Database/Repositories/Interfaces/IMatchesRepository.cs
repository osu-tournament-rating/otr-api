using Database.Entities;
using Database.Enums;
using Database.Enums.Queries;
using Database.Enums.Verification;

namespace Database.Repositories.Interfaces;

public interface IMatchesRepository : IRepository<Match>
{
    /// <summary>
    /// Gets a paged list of matches
    /// </summary>
    /// <remarks>Matches are ordered by primary key</remarks>
    /// <param name="limit">Amount of matches to return. Functions as the "page size"</param>
    /// <param name="page">Which block of matches to return</param>
    /// <returns>A list of matches of size <paramref name="limit"/> indexed by <paramref name="page"/></returns>
    Task<IEnumerable<Match>> GetAsync(
        int limit,
        int page,
        Ruleset? ruleset = null,
        string? name = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        VerificationStatus? verificationStatus = null,
        MatchRejectionReason? rejectionReason = null,
        MatchProcessingStatus? processingStatus = null,
        int? submittedBy = null,
        int? verifiedBy = null,
        MatchesQuerySortType? querySortType = null,
        bool? sortDescending = null
    );

    Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds);

    /// <summary>
    /// Gets a match for the given id including all navigation properties.
    /// </summary>
    /// <remarks>The match and included navigations will not be tracked in the context</remarks>
    /// <param name="id">Match id</param>
    Task<Match?> GetFullAsync(int id);

    Task<IEnumerable<Match>> SearchAsync(string name);

    /// <summary>
    /// Updates the verification status of a match for the given id
    /// </summary>
    /// <param name="id">Id of the match</param>
    /// <param name="verificationStatus">New verification status to assign</param>
    /// <param name="verifierId">Optional user id to attribute the update to</param>
    /// <returns>An updated match, or null if not found</returns>
    Task<Match?> UpdateVerificationStatusAsync(
        int id,
        VerificationStatus verificationStatus,
        int? verifierId = null
    );

    Task<IEnumerable<Match>> GetPlayerMatchesAsync(long osuId, Ruleset ruleset, DateTime before, DateTime after);
}
