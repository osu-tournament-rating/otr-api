using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchScoresRepository : IRepository<MatchScore>
{
	public Task<int> AverageTeammateScore(long osuPlayerId, int mode, DateTime fromTime);
	public Task<int> AverageOpponentScore(long osuPlayerId, int mode, DateTime fromTime);
}