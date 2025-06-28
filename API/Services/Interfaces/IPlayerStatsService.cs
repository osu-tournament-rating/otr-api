using API.DTOs;
using Common.Enums;

namespace API.Services.Interfaces;

public interface IPlayerStatsService
{
    /// <summary>
    /// Retrieves a leaderboard based on the provided query parameters.
    /// </summary>
    /// <param name="request">Parameters used to filter and sort the leaderboard</param>
    Task<LeaderboardDTO> GetLeaderboardAsync(
        LeaderboardRequestQueryDTO request
    );

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
    /// If the player has no data for the given ruleset, the <see cref="PlayerDashboardStatsDTO"/> is returned with
    /// all optional fields null. <see cref="PlayerDashboardStatsDTO.PlayerInfo"/> will always be populated if a
    /// player is found.
    /// </returns>
    Task<PlayerDashboardStatsDTO?> GetAsync(
        string key,
        Ruleset? ruleset = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );

    /// <summary>
    /// Returns the peak ratings for multiple players for a given ruleset and date range.
    /// </summary>
    /// <param name="playerIds">The player ids</param>
    /// <param name="ruleset">The osu! ruleset</param>
    /// <param name="dateMin">The minimum of the date range</param>
    /// <param name="dateMax">The maximum of the date range</param>
    /// <returns>Dictionary mapping player IDs to their peak rating (null if no rating data)</returns>
    Task<Dictionary<int, double?>> GetPeakRatingsAsync(IEnumerable<int> playerIds, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null);

    /// <summary>
    /// Creates a dictionary mapping of player frequencies. The values mapping to 'true' in the result dictionary
    /// are the player's teammates, the values mapped to 'false' are the player's opponents.
    /// </summary>
    /// <param name="playerId">Player id</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Minimum lookup date</param>
    /// <param name="dateMax">Maximum lookup date</param>
    /// <returns></returns>
    Task<Dictionary<bool, List<PlayerFrequencyDTO>>> GetFrequentMatchupsAsync(int playerId, Ruleset ruleset,
        DateTime? dateMin = null, DateTime? dateMax = null);
}
