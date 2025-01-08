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
    /// Checks if the game exists
    /// </summary>
    /// <param name="id">The game id</param>
    /// <returns>True if the game exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Delete a game
    /// </summary>
    /// <param name="id">Game id</param>
    Task DeleteAsync(int id);
}
