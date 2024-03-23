using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchDuplicateRepository : IRepository<MatchDuplicate>
{
    /// <summary>
    ///  Returns all MatchDuplicates where SuspectedDuplicateOf is equal to matchId
    /// </summary>
    /// <param name="matchId"></param>
    /// <returns></returns>
    Task<IEnumerable<MatchDuplicate>> GetDuplicatesAsync(int matchId);
    /// <summary>
    /// Returns all MatchDuplicates where VerifiedAsDuplicate is not true
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<MatchDuplicate>> GetAllUnverifiedAsync();
}
