using API.DTOs;
using Database.Enums;

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
    /// Deletes a match
    /// </summary>
    /// <param name="id">Match id</param>
    Task DeleteAsync(int id);
}
