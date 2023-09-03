using API.Entities;

namespace API.Services.Interfaces;

public interface IMatchScoresService : IService<MatchScore>
{
	Task<IEnumerable<MatchScore>> GetForGameAsync(long gameId);
	Task<IEnumerable<MatchScore>> GetForPlayerAsync(long playerId);
	Task<int> BulkInsertAsync(IEnumerable<MatchScore> matchScores);
	Task<int?> CreateIfNotExistsAsync(MatchScore dbScore);
}