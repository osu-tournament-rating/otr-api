using API.DTOs;
using Database.Enums;

namespace API.Services.Interfaces;

public interface IPlayerStatsService
{
    /// <summary>
    /// Gets player stats for the given ruleset in the given date range
    /// </summary>
    /// <remarks>
    /// Gets player by versatile search.
    /// If no ruleset is provided, the player's default is used. <see cref="Ruleset.Osu"/> is used as a fallback
    /// If no date range is provided, gets all stats without considering date
    /// </remarks>
    /// <param name="key">Key used in versatile search</param>
    /// <param name="ruleset">Ruleset to filter for</param>
    /// <param name="dateMin">Start of date range</param>
    /// <param name="dateMax">End of date range</param>
    /// <returns>
    /// Complete player statistics, or null if no player is found for the given key.
    /// If the player has no data for the given ruleset, the <see cref="PlayerStatsDTO"/> is returned with
    /// all optional fields null. <see cref="PlayerStatsDTO.PlayerInfo"/> will always be populated if a
    /// player is found.
    /// </returns>
    Task<PlayerStatsDTO?> GetAsync(
        string key,
        Ruleset? ruleset = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );

    Task<PlayerTeammateComparisonDTO> GetTeammateComparisonAsync(
        int playerId,
        int teammateId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    );

    Task<PlayerOpponentComparisonDTO> GetOpponentComparisonAsync(
        int playerId,
        int opponentId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    );

    /// <summary>
    /// Returns the peak rating of a player for a given ruleset and date range.
    /// </summary>
    /// <param name="playerId">The player id</param>
    /// <param name="ruleset">The osu! ruleset</param>
    /// <param name="dateMin">The minimum of the date range</param>
    /// <param name="dateMax">The maximum of the date range</param>
    /// <returns></returns>
    Task<double> GetPeakRatingAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null);
}
