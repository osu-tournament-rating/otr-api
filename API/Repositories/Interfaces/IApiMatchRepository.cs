using API.Osu.Multiplayer;
using Database.Entities;

namespace API.Repositories.Interfaces;

public interface IApiMatchRepository
{
    /// <summary>
    /// Inserts necessary data into the database from an osu! API match result.
    /// </summary>
    /// <param name="apiMatch">A single osu! match as represented from the API v1 wiki: https://github.com/ppy/osu-api/wiki#apiget_match</param>
    /// <returns>The inserted match, or null if the match cannot be processed (likely due to not passing automated checks).</returns>
    Task<Match?> CreateFromApiMatchAsync(OsuApiMatchData apiMatch);
}
