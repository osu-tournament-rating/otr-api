using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface ITournamentsRepository : IRepository<Tournament>
{
    public Task<Tournament?> GetAsync(string name);

    /// <summary>
    /// Creates a tournament from a web submission
    /// </summary>
    /// <param name="wrapper">The user input required for this tournament</param>
    /// <returns></returns>
    public Task<Tournament> CreateAsync(TournamentWebSubmissionDTO wrapper);

    Task<int> CountPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null);
    public Task<bool> ExistsAsync(string name, int mode);

    public Task<PlayerTournamentTeamSizeCountDTO> GetPlayerTeamSizeStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    );

    /// <summary>
    ///  Finds and returns the best or worst tournaments for a player, rated and ordered by average match cost.
    /// </summary>
    /// <param name="count">The number of tournaments to return</param>
    /// <returns>A list of <see cref="count" /> tournaments ordered by the player's average match cost, descending</returns>
    Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(
        int count,
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax,
        bool bestPerformances
    );

    /// <summary>
    /// Updates a tournament from a web submission
    /// </summary>
    /// <param name="wrapper">The user input required for this tournament</param>
    /// <param name="id">Id of the target tournament</param>
    /// <returns></returns>
    public Task<Tournament> UpdateAsync(int id, TournamentDTO wrapper);
}
