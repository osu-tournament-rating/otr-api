using API.Models;

namespace API.Services.Interfaces;

public interface IMatchScoresService : IService<MatchScore>
{
	Task<IEnumerable<MatchScore>> GetForGameAsync(int gameId);
	Task<IEnumerable<MatchScore>> GetForPlayerAsync(int playerId);
	Task<int> BulkInsertAsync(IEnumerable<MatchScore> matchScores);
	Task<int?> CreateIfNotExistsAsync(MatchScore dbScore);
}