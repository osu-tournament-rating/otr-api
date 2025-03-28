using Common.Enums;
using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IMatchRosterRepository : IRepository<MatchRoster>
{
    /// <summary>
    /// Fetch all rosters known to this player
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="ruleset"></param>
    /// <param name="dateMin"></param>
    /// <param name="dateMax"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    Task<IEnumerable<MatchRoster>> FetchRostersAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        int limit = 5
    );
}
