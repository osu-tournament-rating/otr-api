using API.DTOs;
using Common.Enums;

namespace API.Services.Interfaces;

public interface IMatchesService
{
    /// <summary>
    /// Gets a paged list of matches
    /// </summary>
    Task<IEnumerable<MatchDTO>> GetAsync(MatchRequestQueryDTO filter);

    /// <summary>
    /// Gets a match for the given id including all navigation properties.
    /// </summary>
    /// <param name="id">Match id</param>
    Task<MatchDTO?> GetAsync(int id);

    Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        Ruleset ruleset,
        DateTime start,
        DateTime end
    );

    /// <summary>
    /// Searches for the specified match by name
    /// </summary>
    /// <param name="name">The partial search key for the match name</param>
    /// <returns>Up to 30 results if any matches are found</returns>
    Task<IEnumerable<MatchSearchResultDTO>> SearchAsync(string name);

    /// <summary>
    /// Updates a match entity with values from a <see cref="MatchDTO" />
    /// </summary>
    /// <param name="id">The match id</param>
    /// <param name="match">The DTO containing the new values</param>
    /// <returns>The updated DTO</returns>
    Task<MatchDTO?> UpdateAsync(int id, MatchDTO match);

    /// <summary>
    /// Checks if the match exists
    /// </summary>
    /// <param name="id">The match id</param>
    /// <returns>True if the match exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Links the games of each provided match to the parent match, then deletes the provided matches
    /// </summary>
    /// <param name="matchIds">
    /// Ids of the matches to merge into the parent match
    /// </param>
    /// <returns>
    /// The updated parent match with child navigations, or null if the parent could not be found.
    /// If this method returns null, no data is modified.
    /// </returns>
    Task<MatchDTO?> MergeAsync(int parentId, IEnumerable<int> matchIds);

    /// <summary>
    /// Deletes a match
    /// </summary>
    /// <param name="id">Match id</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Deletes all scores belonging to a player for a given match
    /// </summary>
    /// <param name="matchId">Match id</param>
    /// <param name="playerId">Player id</param>
    /// <returns>The number of scores deleted</returns>
    Task<int> DeletePlayerScoresAsync(int matchId, int playerId);
}
