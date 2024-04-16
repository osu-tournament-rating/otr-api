using API.Entities;
using API.Osu;

namespace API.Repositories.Interfaces;

public interface IMatchScoresRepository : IRepository<MatchScore>
{
    Task<int> AverageTeammateScoreAsync(long osuPlayerId, OsuEnums.Ruleset ruleset, DateTime fromTime);
    Task<int> AverageOpponentScoreAsync(long osuPlayerId, OsuEnums.Ruleset ruleset, DateTime fromTime);
    Task<int> AverageModScoreAsync(int playerId, OsuEnums.Ruleset ruleset, int mods, DateTime dateMin, DateTime dateMax);

    Task<int> CountModScoresAsync(int playerId, OsuEnums.Ruleset ruleset, int mods, DateTime dateMin, DateTime dateMax);
}
