using API.DTOs;

namespace API.Services.Interfaces;

public interface IGameScoresService
{
    /// <summary>
    ///  Gets a score by id
    /// </summary>
    /// <param name="id">The id of the score</param>
    /// <returns>The score, or null if not found</returns>
    Task<GameScoreDTO?> GetAsync(int id);

    /// <summary>
    ///  Updates a score entity with values from a <see cref="GameScoreDTO" />
    /// </summary>
    /// <param name="id">The score id</param>
    /// <param name="match">The DTO containing the new values</param>
    /// <returns>The updated DTO</returns>
    Task<GameScoreDTO?> UpdateAsync(int id, GameScoreDTO score);
}