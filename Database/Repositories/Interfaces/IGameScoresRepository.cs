using Database.Entities;
using Database.Enums;

namespace Database.Repositories.Interfaces;

public interface IGameScoresRepository : IRepository<GameScore>
{
    Task<int> AverageTeammateScoreAsync(long osuPlayerId, Ruleset ruleset, DateTime fromTime);
    Task<int> AverageOpponentScoreAsync(long osuPlayerId, Ruleset ruleset, DateTime fromTime);
    Task<int> AverageModScoreAsync(int playerId, Ruleset ruleset, int mods, DateTime dateMin, DateTime dateMax);

    Task<int> CountModScoresAsync(int playerId, Ruleset ruleset, int mods, DateTime dateMin, DateTime dateMax);
}
