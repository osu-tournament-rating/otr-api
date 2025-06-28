using API.DTOs;
using Common.Enums;
using Database.Entities;

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
    /// Gets the number of tournaments played by the given player
    /// </summary>
    Task<int> CountPlayedAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null);

    /// <summary>
    /// Gets the number of tournaments played by multiple players
    /// </summary>
    /// <param name="playerIds">The player ids</param>
    /// <param name="ruleset">The ruleset</param>
    /// <param name="dateMin">Minimum date filter</param>
    /// <param name="dateMax">Maximum date filter</param>
    /// <returns>Dictionary mapping player IDs to their tournament count</returns>
    Task<Dictionary<int, int>> CountPlayedAsync(IEnumerable<int> playerIds, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null);

    /// <summary>
    /// Updates a tournament entity with values from a <see cref="TournamentDTO"/>
    /// </summary>
    Task<TournamentCompactDTO?> UpdateAsync(int id, TournamentCompactDTO wrapper);

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
    /// <param name="verifierUserId">User id of the verifier</param>
    /// <returns>The updated <see cref="TournamentDTO"/></returns>
    Task<TournamentDTO?> AcceptPreVerificationStatusesAsync(int id, int verifierUserId);

    /// <summary>
    /// Adds a collection of osu! beatmap ids to the tournament's PooledBeatmaps collection
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="osuBeatmapIds">A collection of osu! beatmap ids to add</param>
    /// <returns>
    /// The <see cref="Tournament"/>'s <see cref="Tournament.PooledBeatmaps"/> collection or an
    /// empty collection if the tournament does not exist
    /// </returns>
    Task<ICollection<BeatmapDTO>> AddPooledBeatmapsAsync(int id, ICollection<long> osuBeatmapIds);

    /// <summary>
    /// Gets the <see cref="Tournament"/>'s <see cref="Tournament.PooledBeatmaps"/> collection
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <returns>
    /// The <see cref="Tournament"/>'s <see cref="Tournament.PooledBeatmaps"/> collection or an
    /// empty collection if the tournament does not exist
    /// </returns>
    Task<ICollection<BeatmapDTO>> GetPooledBeatmapsAsync(int id);

    /// <summary>
    /// Unmaps the provided beatmap ids from being pooled in the given tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="beatmapIds">Collection of beatmap ids to remove from the tournament's collection of pooled beatmaps</param>
    Task DeletePooledBeatmapsAsync(int id, ICollection<int> beatmapIds);

    /// <summary>
    /// Removes all pooled beatmaps from a given tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    Task DeletePooledBeatmapsAsync(int id);

    /// <summary>
    /// Reruns automation checks for a tournament and all child entities
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="force">Whether to overwrite fully Verified/Rejected data (dangerous)</param>
    Task RerunAutomationChecksAsync(int id, bool force = false);
}
