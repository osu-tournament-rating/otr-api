using API.DTOs;

namespace API.Services.Interfaces;

public interface IGamesService
{
    /// <summary>
    ///  Gets a game by id
    /// </summary>
    /// <param name="id">The id of the game</param>
    /// <param name="verified">Whether the game's scores must be verified</param>
    /// <returns>The game, or null if not found</returns>
    Task<GameDTO?> GetAsync(int id, bool verified);

    /// <summary>
    ///  Updates a game entity with values from a <see cref="GameDTO" />
    /// </summary>
    /// <param name="id">The game id</param>
    /// <param name="match">The DTO containing the new values</param>
    /// <returns>The updated DTO</returns>
    Task<GameDTO?> UpdateAsync(int id, GameDTO game);

    /// <summary>
    /// Delete a game
    /// </summary>
    /// <param name="id">Game id</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Merges scores from source games into a target game.
    /// The source games must be from the same match and have the same beatmap as the target game.
    /// After merging, the source games are deleted.
    /// </summary>
    /// <param name="targetGameId">The ID of the game to merge scores into</param>
    /// <param name="sourceGameIds">The IDs of the games whose scores will be merged</param>
    /// <returns>The merged game if successful, null if failed</returns>
    Task<GameDTO?> MergeScoresAsync(int targetGameId, IEnumerable<int> sourceGameIds);
}
