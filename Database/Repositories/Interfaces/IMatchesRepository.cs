using Database.Entities;
using Database.Enums;

namespace Database.Repositories.Interfaces;

public interface IMatchesRepository : IRepository<Match>
{
    Task<Match?> GetAsync(int id, bool filterInvalidMatches = true);

    /// <summary>
    /// Gets a paged list of matches
    /// </summary>
    /// <remarks>
    /// Matches are ordered by primary key
    /// </remarks>
    /// <param name="limit">Amount of matches to return. Functions as the "page size"</param>
    /// <param name="page">Which block of matches to return</param>
    /// <param name="filterUnverified">If unverified matches should be excluded from the results</param>
    /// <returns>A list of matches of size <paramref name="limit"/> indexed by <paramref name="page"/></returns>
    Task<IEnumerable<Match>> GetAsync(int limit, int page, bool filterUnverified = true);

    Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds);
    Task<Match?> GetByMatchIdAsync(long matchId);
    Task<IEnumerable<Match>> SearchAsync(string name);
    Task<IList<Match>> GetMatchesNeedingAutoCheckAsync(int limit = 10000);
    Task<Match?> GetFirstMatchNeedingApiProcessingAsync();

    /// <summary>
    /// Updates the verification status of a match for the given id
    /// </summary>
    /// <param name="id">Id of the match</param>
    /// <param name="verificationStatus">New verification status to assign</param>
    /// <param name="verificationSource">New verification source to assign</param>
    /// <param name="info">Optional verification info</param>
    /// <param name="verifierId">Optional user id to attribute the update to</param>
    /// <returns>An updated match, or null if not found</returns>
    Task<Match?> UpdateVerificationStatusAsync(
        int id,
        Old_MatchVerificationStatus status,
        Old_MatchVerificationSource source,
        string? info = null,
        int? verifierId = null
    );
    Task<IEnumerable<Match>> GetPlayerMatchesAsync(long osuId, int mode, DateTime before, DateTime after);
    Task UpdateAsApiProcessed(Match match);
    Task SetRequireAutoCheckAsync(bool invalidOnly = true);
}
