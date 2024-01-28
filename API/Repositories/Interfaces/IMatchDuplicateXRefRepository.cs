using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchDuplicateXRefRepository : IRepository<MatchDuplicateXRef>
{
	Task<IEnumerable<MatchDuplicateXRef>> GetDuplicatesAsync(int matchId);
}