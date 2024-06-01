using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IMatchScoresRepository : IRepository<MatchScore>
{
    Task<int> AverageTeammateScoreAsync(long osuPlayerId, Ruleset ruleset, DateTime fromTime);
    Task<int> AverageOpponentScoreAsync(long osuPlayerId, Ruleset ruleset, DateTime fromTime);
    Task<int> AverageModScoreAsync(int playerId, Ruleset ruleset, int mods, DateTime dateMin, DateTime dateMax);

    Task<int> CountModScoresAsync(int playerId, Ruleset ruleset, int mods, DateTime dateMin, DateTime dateMax);
}
