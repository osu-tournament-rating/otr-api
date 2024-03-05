using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchScoresRepository : IRepository<MatchScore>
{
    Task<int> AverageTeammateScoreAsync(long osuPlayerId, int mode, DateTime fromTime);
    Task<int> AverageOpponentScoreAsync(long osuPlayerId, int mode, DateTime fromTime);
    Task<int> AverageModScoreAsync(int playerId, int mode, int mods, DateTime dateMin, DateTime dateMax);

    Task<int> CountModScoresAsync(int playerId, int mode, int mods, DateTime dateMin, DateTime dateMax);
}
