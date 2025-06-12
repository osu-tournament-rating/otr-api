using API.DTOs;
using Common.Enums;

namespace API.Services.Interfaces;

public interface IPlayerService
{
    /// <summary>
    /// Searches for a player using the provided key.
    /// Priority is given to numerical keys, attempting to interpret them as either a player ID or an osu! ID (in that order).
    /// If the key is not a number, a case-insensitive username search is performed.
    /// </summary>
    /// <param name="key">The search key for identifying the player</param>
    /// <returns>Returns null if no matching player is found</returns>
    Task<PlayerCompactDTO?> GetVersatileAsync(string key);

    /// <summary>
    /// Retrieves player information for a collection of osu! IDs
    /// </summary>
    /// <param name="osuIds">A collection of osu! player IDs to look up</param>
    /// <returns>
    /// A collection of <see cref="PlayerCompactDTO"/> objects.
    /// Players who cannot be identified will not be in the returned collection.
    /// </returns>
    Task<IEnumerable<PlayerCompactDTO>> GetAsync(IEnumerable<long> osuIds);

    /// <summary>
    /// Retrieves all tournaments that a player has participated in
    /// </summary>
    /// <param name="key">The search key for identifying the player</param>
    /// <param name="ruleset">Optional ruleset to filter tournaments. If null, returns tournaments from all rulesets</param>
    /// <param name="dateMin">Optional minimum date filter. If null, no lower date limit is applied</param>
    /// <param name="dateMax">Optional maximum date filter. If null, no upper date limit is applied</param>
    /// <returns>
    /// A collection of <see cref="TournamentCompactDTO"/> objects representing tournaments the player has participated in.
    /// Returns null if the player is not found.
    /// </returns>
    Task<IEnumerable<TournamentCompactDTO>?> GetTournamentsAsync(string key, Ruleset? ruleset = null, DateTime? dateMin = null, DateTime? dateMax = null);
}
