using API.DTOs;

namespace API.Services.Interfaces;

public interface IGamesService
{
    /// <summary>
    ///  Gets a game by id
    /// </summary>
    /// <param name="id">The id of the game</param>
    /// <returns>The game, or null if not found</returns>
    Task<GameDTO?> GetAsync(int id);

    /// <summary>
    ///  Updates a game entity with values from a <see cref="GameDTO" />
    /// </summary>
    /// <param name="id">The game id</param>
    /// <param name="match">The DTO containing the new values</param>
    /// <returns>The updated DTO</returns>
    Task<GameDTO?> UpdateAsync(int id, GameDTO game);
}
