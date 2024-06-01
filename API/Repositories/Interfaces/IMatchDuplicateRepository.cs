using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IMatchDuplicateRepository : IRepository<MatchDuplicate>
{
    Task<IEnumerable<MatchDuplicate>> GetDuplicatesAsync(int matchId);
    Task<IEnumerable<MatchDuplicate>> GetAllUnknownStatusAsync();
}
