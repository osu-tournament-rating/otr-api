using API.DTOs;
using API.Enums;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiTournamentsRepository : ITournamentsRepository
{
    /// <summary>
    /// Search for a tournament by name
    /// </summary>
    public Task<IEnumerable<TournamentSearchResultDTO>> SearchAsync(string name);

    /// <summary>
    /// Create team size statistics for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    public Task<PlayerTournamentLobbySizeCountDTO> GetLobbySizeStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    );
}
