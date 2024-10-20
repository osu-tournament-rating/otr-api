using API.Enums;
using Database.Entities;
using Database.Enums;

namespace Database.Repositories.Interfaces;

public interface ITournamentsRepository : IRepository<Tournament>
{
    /// <summary>
    /// Gets a <see cref="Tournament"/> by id
    /// </summary>
    /// <param name="id">The tournament id</param>
    /// <param name="eagerLoad">Whether to eagerly load navigational properties</param>
    Task<Tournament?> GetAsync(int id, bool eagerLoad = false);

    /// <summary>
    /// Gets a <see cref="Tournament" /> by id with verified child navigations
    /// </summary>
    /// <param name="id">The id of the tournament</param>
    /// <returns>
    /// Null if the tournament is not found.
    /// Returns a tournament with verified child navigations if found.
    /// </returns>
    Task<Tournament?> GetVerifiedAsync(int id);

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

    /// <summary>
    /// Gets all tournaments with pagination
    /// </summary>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The size of the collection</param>
    /// <param name="sortKey">Determines how the results are sorted</param>
    /// <param name="verified">Whether the resulting tournaments must be verified and
    /// have completed processing</param>
    /// <param name="ruleset">An optional ruleset to filter by</param>
    Task<ICollection<Tournament>> GetAsync(int page, int pageSize, TournamentSortKey sortKey,
        bool verified = true, Ruleset? ruleset = null);
}
