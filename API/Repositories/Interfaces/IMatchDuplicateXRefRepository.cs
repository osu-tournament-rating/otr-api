using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchDuplicateXRefRepository : IRepository<MatchDuplicate>
{
	Task<IEnumerable<MatchDuplicate>> GetDuplicatesAsync(int matchId);
}