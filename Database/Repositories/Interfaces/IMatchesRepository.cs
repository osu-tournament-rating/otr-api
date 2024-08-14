using Database.Entities;
using Database.Enums;
using Database.Enums.Queries;
using Database.Enums.Verification;

namespace Database.Repositories.Interfaces;

public interface IMatchesRepository : IRepository<Match>
{
    /// <summary>
    /// Gets a match with children for the given id
    /// </summary>
    /// <param name="id">Match id</param>
    /// <param name="filterType">Determines the way matches and children are filtered</param>
    /// <returns>A match, or null if one does not exist</returns>
    Task<Match?> GetAsync(
        int id,
        QueryFilterType filterType = QueryFilterType.Verified | QueryFilterType.ProcessingCompleted
    );

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

    /// <summary>
    /// Gets a paged list of matches
    /// </summary>
    /// <remarks>Matches are ordered by primary key</remarks>
    /// <param name="limit">Amount of matches to return. Functions as the "page size"</param>
    /// <param name="page">Which block of matches to return</param>
    /// <param name="filterType">Determines the way matches are filtered</param>
    /// <returns>A list of matches of size <paramref name="limit"/> indexed by <paramref name="page"/></returns>
    Task<IEnumerable<Match>> GetAsync(
        int limit,
        int page,
        QueryFilterType filterType = QueryFilterType.Verified | QueryFilterType.ProcessingCompleted
    );

    Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds);

    Task<Match?> GetByOsuIdAsync(long osuId);

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
    Task<IEnumerable<Match>> GetPlayerMatchesAsync(long osuId, int mode, DateTime before, DateTime after);
}
