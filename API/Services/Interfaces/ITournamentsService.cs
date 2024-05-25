using API.DTOs;

namespace API.Services.Interfaces;

public interface ITournamentsService
{
    /// <summary>
    /// Creates a tournament from a <see cref="TournamentWebSubmissionDTO"/>.
    /// </summary>
    /// <param name="wrapper">Submission data</param>
    /// <param name="verify">Verify all included matches</param>
    /// <param name="verificationSource">Source of verification (int representation of <see cref="Database.Enums.MatchVerificationSource"/></param>
    /// <returns>Location information for the created tournament</returns>
    Task<TournamentCreatedResultDTO> CreateAsync(TournamentWebSubmissionDTO wrapper, bool verify, int? verificationSource);

    /// <summary>
    /// Denotes a tournament with matching id exists
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Denotes a tournament with matching name and mode exists
    /// </summary>
    Task<bool> ExistsAsync(string name, int mode);

    /// <summary>
    /// Gets all tournaments
    /// </summary>
    Task<IEnumerable<TournamentDTO>> ListAsync();

    /// <summary>
    /// Gets a tournament by id
    /// </summary>
    /// <param name="id">Primary key</param>
    /// <param name="eagerLoad">Whether to include child resources of the tournament</param>
    /// <returns>The tournament, or null if not found</returns>
    Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true);

    /// <summary>
    /// Gets the number of tournaments played by the given player
    /// </summary>
    Task<int> CountPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null);

    /// <summary>
    /// Updates a tournament entity with values from a <see cref="TournamentDTO"/>
    /// </summary>
    Task<TournamentDTO?> UpdateAsync(int id, TournamentDTO wrapper);
}
