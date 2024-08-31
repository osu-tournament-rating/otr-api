using Database.Entities;
using Database.Enums;
using Database.Queries.Filters;

namespace Database.Repositories.Interfaces;

public interface ITournamentsRepository : IRepository<Tournament>
{
    /// <summary>
    /// Get a <see cref="Tournament"/> entity
    /// </summary>
    /// <param name="id">Primary key</param>
    /// <param name="eagerLoad">Whether to eagerly load navigational properties</param>
    Task<Tournament?> GetAsync(int id, bool eagerLoad = false);

    /// <summary>
    /// Get a collection of tournaments
    /// </summary>
    /// <param name="limit">Amount of entities to return. Functions as the "page size"</param>
    /// <param name="page">Which block of entities to return</param>
    /// <param name="filter">Filtering options</param>
    /// <param name="tracked">Denotes if the entities should be added to the Change Tracker</param>
    Task<IEnumerable<Tournament>> GetAsync(
        int limit,
        int page,
        TournamentsQueryFilter filter,
        bool tracked = true
    );

    /// <summary>
    /// Gets tournaments with a <see cref="Enums.Verification.TournamentProcessingStatus"/>
    /// that is not <see cref="Enums.Verification.TournamentProcessingStatus.Done"/>
    /// </summary>
    /// <param name="limit">Maximum number of tournaments</param>
    Task<IEnumerable<Tournament>> GetNeedingProcessingAsync(int limit);

    /// <summary>
    /// Denotes if a tournament with the given name and ruleset exists
    /// </summary>
    public Task<bool> ExistsAsync(string name, Ruleset ruleset);

    /// <summary>
    /// Count number of tournaments played for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    Task<int> CountPlayedAsync(int playerId, Ruleset ruleset, DateTime dateMin, DateTime dateMax);
}
