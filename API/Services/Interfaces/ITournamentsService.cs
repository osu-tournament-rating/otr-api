using API.DTOs;
using Database.Enums;

namespace API.Services.Interfaces;

public interface ITournamentsService
{
    /// <summary>
    /// Creates a tournament from a <see cref="TournamentSubmissionDTO"/>.
    /// </summary>
    /// <param name="submission">Tournament submission data</param>
    /// <param name="submitterUserId">Id of the User that created the submission</param>
    /// <param name="preApprove">Denotes if the tournament should be pre-approved</param>
    /// <returns>Location information for the created tournament</returns>
    Task<TournamentCreatedResultDTO> CreateAsync(
        TournamentSubmissionDTO submission,
        int submitterUserId,
        bool preApprove
    );

    /// <summary>
    /// Denotes a tournament exists for the given id
    /// </summary>
    /// <param name="id">Tournament id</param>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Denotes a tournament with matching name and ruleset exists
    /// </summary>
    Task<bool> ExistsAsync(string name, Ruleset ruleset);

    /// <summary>
    /// Gets all tournaments
    /// </summary>
    Task<IEnumerable<TournamentDTO>> ListAsync();

    /// <summary>
    /// Gets a tournament by id
    /// </summary>
    /// <param name="id">The tournament id</param>
    /// <param name="eagerLoad">Whether to include child resources of the tournament</param>
    /// <returns>The tournament, or null if not found</returns>
    Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true);

    /// <summary>
    /// Gets a collection of tournaments that match the provided query
    /// </summary>
    /// <param name="requestQuery">The tournament request query</param>
    /// <returns>A collection of <see cref="TournamentDTO"/>s which align with the request query</returns>
    Task<ICollection<TournamentDTO>> GetAsync(TournamentRequestQueryDTO requestQuery);

    /// <summary>
    /// Gets a verified tournament that matches the provided id. All child navigations are
    /// verified.
    /// </summary>
    /// <param name="id">The id of the verified tournament</param>
    /// <returns>
    /// Null if the tournament is not found.
    /// Returns a tournament with verified child navigations if found.
    /// </returns>
    Task<TournamentDTO?> GetVerifiedAsync(int id);

    /// <summary>
    /// Gets the number of tournaments played by the given player
    /// </summary>
    Task<int> CountPlayedAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null);

    /// <summary>
    /// Updates a tournament entity with values from a <see cref="TournamentDTO"/>
    /// </summary>
    Task<TournamentDTO?> UpdateAsync(int id, TournamentDTO wrapper);

    /// <summary>
    /// Deletes a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// If the tournament is pre-rejected or pre-verified, updates the tournament
    /// to be rejected or verified respectively. This update strategy is applied
    /// to all child <see cref="Match"/>es, <see cref="Game"/>s, and
    /// <see cref="GameScore"/>s
    /// </summary>
    /// <param name="id">Tournament id</param>
    Task AcceptVerificationStatusesAsync(int id);
}
