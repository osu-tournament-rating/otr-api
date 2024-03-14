using API.DTOs;

namespace API.Services.Interfaces;

public interface ITournamentsService
{
    /// <summary>
    /// Creates or udpates a tournament from a web submission.
    /// </summary>
    /// <param name="wrapper">The user input required for this tournament</param>
    /// <param name="updateExisting">Whether to overwrite values for an existing occurrence of this tournament</param>
    /// <returns></returns>
    public Task<TournamentDTO> CreateOrUpdateAsync(
        TournamentWebSubmissionDTO wrapper,
        bool updateExisting = false
    );

    public Task<bool> ExistsAsync(string name, int mode);
    Task<IEnumerable<TournamentDTO>> GetAllAsync();
    Task<TournamentDTO?> GetByName(string name);

    /// <summary>
    ///  Counts the number of tournaments played by the given player.
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<int> CountPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null);
}
