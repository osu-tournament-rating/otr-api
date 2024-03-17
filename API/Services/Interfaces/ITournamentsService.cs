using API.DTOs;

namespace API.Services.Interfaces;

public interface ITournamentsService
{
    /// <summary>
    /// Create a tournament from a web submission.
    /// </summary>
    /// <param name="wrapper">The user input required for this tournament</param>
    /// <returns></returns>
    Task<TournamentDTO> CreateAsync(TournamentWebSubmissionDTO wrapper);

    /// <summary>
    /// Does a tournament with matching <paramref name="id"/> exist
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <returns></returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Does a tournament with matching <paramref name="name"/> and <paramref name="mode"/> exist
    /// </summary>
    /// <param name="name">Tournament name</param>
    /// <param name="mode">Tournament mode</param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string name, int mode);

    /// <summary>
    /// Get all tournaments
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<TournamentDTO>> GetAllAsync();

    /// <summary>
    /// Get a tournament by id, returns null if not found
    /// </summary>
    /// <param name="id">Primary key</param>
    /// <param name="eagerLoad">Whether to include child resources of the tournament</param>
    /// <returns></returns>
    Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true);

    /// <summary>
    /// Counts the number of tournaments played by the given player.
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<int> CountPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null);

    /// <summary>
    /// Updates a database entity based on values from a DTO
    /// </summary>
    /// <param name="wrapper">User provided values</param>
    /// <param name="id">Id of the target tournament</param>
    /// <returns></returns>
    Task<TournamentDTO> UpdateAsync(int id, TournamentDTO wrapper);
}
