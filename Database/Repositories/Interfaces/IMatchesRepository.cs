using Common.Enums;
using Common.Enums.Queries;
using Common.Enums.Verification;
using Database.Entities;

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
        MatchQuerySortType? querySortType = null,
        bool? sortDescending = null
    );

    Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds);

    /// <summary>
    /// Gets a match for the given id including all navigation properties.
    /// </summary>
    /// <remarks>The match and included navigations will not be tracked in the context</remarks>
    /// <param name="id">Match id</param>
    /// <param name="verified">Whether the match (and all child navigations) are verified</param>
    Task<Match?> GetFullAsync(int id, bool verified);

    /// <summary>
    /// Searches for matches by name
    /// </summary>
    /// <param name="name">Name to search for</param>
    /// <returns>A list of matches that match the name</returns>
    Task<IEnumerable<Match>> SearchAsync(string name);

    /// <summary>
    /// Links the games of each provided match to the parent match, then deletes the provided matches
    /// </summary>
    /// <remarks>
    /// Specifically, for each game of each match provided in <see cref="matchIds"/>,
    /// the Match property is reassigned to the match whose ID equals <see cref="parentId"/>.
    /// After the linking happens, each match provided in <see cref="matchIds"/> is deleted.
    /// </remarks>
    /// <param name="matchIds">
    /// Ids of the matches to merge into the parent match
    /// </param>
    /// <returns>
    /// The updated parent match with child navigations, or null if the parent could not be found.
    /// If this method returns null, no data is modified.
    /// </returns>
    Task<Match?> MergeAsync(int parentId, IEnumerable<int> matchIds);

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
