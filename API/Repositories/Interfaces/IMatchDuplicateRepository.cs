using Database.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchDuplicateRepository : IRepository<MatchDuplicate>
{
    Task<IEnumerable<MatchDuplicate>> GetDuplicatesAsync(int matchId);
    Task<IEnumerable<MatchDuplicate>> GetAllUnknownStatusAsync();
}
