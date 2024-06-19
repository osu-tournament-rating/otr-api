using API.DTOs;
using API.Entities;
using API.Enums;
using API.Osu.Enums;

namespace API.Repositories.Interfaces;

public interface ITournamentsRepository : IRepository<Tournament>
{
    /// <summary>
    /// Get a <see cref="Tournament"/> entity
    /// </summary>
    /// <param name="id">Primary key</param>
    /// <param name="eagerLoad">Whether to eagerly load navigational properties</param>
    Task<Tournament?> GetAsync(int id, bool eagerLoad = false);

    /// <summary>
    /// Returns whether an entity with the given name and mode exists
    /// </summary>
    public Task<bool> ExistsAsync(string name, int mode);

    /// <summary>
    /// Search for a tournament by name
    /// </summary>
    public Task<IEnumerable<TournamentSearchResultDTO>> SearchAsync(string name);

    /// <summary>
    /// Create team size statistics for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="mode">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    public Task<PlayerTournamentTeamSizeCountDTO> GetTeamSizeStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    );

    /// <summary>
    /// Gets a list of tournament performances for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="ruleset">Ruleset of the tournaments</param>
    /// <param name="dateMin">Date range lower bound</param>
    /// <param name="dateMax">Date range upper bound</param>
    /// <param name="resultType">Denotes the manner in which results are ordered</param>
    /// <param name="limit">Number of performances</param>
    /// <returns>
    /// A list of <see cref="PlayerTournamentMatchCostDTO"/> of size <paramref name="limit"/> for tournaments in
    /// <paramref name="ruleset"/> with timestamps between the <paramref name="dateMin"/> and <paramref name="dateMax"/>
    /// ordered by the <see cref="resultType"/>
    /// </returns>
    Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax,
        TournamentPerformanceResultType resultType,
        int limit = 5
    );

    /// <summary>
    /// Count number of tournaments played for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="mode">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    Task<int> CountPlayedAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
}
