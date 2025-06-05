using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface ITournamentsRepository : IRepository<Tournament>
{
    /// <summary>
    /// Gets a <see cref="Tournament"/> by id
    /// </summary>
    /// <param name="id">The tournament id</param>
    /// <param name="eagerLoad">
    /// Whether to eagerly load navigational properties.
    /// If true, all returned entities will not be tracked by the context
    /// </param>
    /// <remarks>
    /// <see cref="Entities.PlayerTournamentStats"/> will be included for this tournament,
    /// the <see cref="Entities.PlayerMatchStats"/> will be included for matches played in this tournament.
    /// </remarks>
    Task<Tournament?> GetAsync(int id, bool eagerLoad = false);

    /// <summary>
    /// Gets a <see cref="Tournament" /> by id with verified child navigations
    /// </summary>
    /// <param name="id">The id of the tournament</param>
    /// <returns>
    /// Null if the tournament is not found.
    /// Returns a tournament with verified child navigations if found.
    /// </returns>
    /// <remarks>All returned entities will not be tracked by the context</remarks>
    Task<Tournament?> GetVerifiedAsync(int id);

    /// <summary>
    /// Gets tournaments with a <see cref="TournamentProcessingStatus"/>
    /// that is not <see cref="TournamentProcessingStatus.Done"/>
    /// </summary>
    /// <param name="limit">Maximum number of tournaments</param>
    Task<IEnumerable<Tournament>> GetNeedingProcessingAsync(int limit);

    /// <summary>
    /// Denotes if a tournament with the given name and ruleset exists
    /// </summary>
    public Task<bool> ExistsAsync(string name, Ruleset ruleset);

    /// <summary>
    /// Count number of tournaments played for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    Task<int> CountPlayedAsync(int playerId, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax);

    /// <summary>
    /// Gets all tournaments with pagination
    /// </summary>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The size of the collection</param>
    /// <param name="querySortType">Determines how the results are sorted</param>
    /// <param name="verified">
    /// Filters results for only tournaments that are verified
    /// see <see cref="ITournamentsRepository.GetVerifiedAsync"/>
    /// </param>
    /// <param name="ruleset">Filters results for only tournaments played in a specified ruleset
    /// </param>
    /// <param name="searchQuery">
    /// Filters results for only tournaments with a partially matching name or abbreviation (case insensitive)
    /// </param>
    /// <param name="dateMin"
    /// >Filters results for only tournaments that occurred on or after a specified date
    /// </param>
    /// <param name="dateMax">
    /// Filters results for only tournaments that occurred on or before a specified date
    /// </param>
    /// <param name="verificationStatus">
    /// Filters results for only tournaments with a specified verification status
    /// </param>
    /// <param name="rejectionReason">
    /// Filters results for only tournaments with a specified rejection reason
    ///</param>
    /// <param name="processingStatus">
    /// Filters results for only tournaments with a specified processing status
    /// </param>
    /// <param name="submittedBy">
    /// Filters results for only tournaments submitted by a user with a specified id
    /// </param>
    /// <param name="verifiedBy">
    /// Filters results for only tournaments verified by a user with a specified id
    /// </param>
    /// <param name="lobbySize">
    /// Filters results for only tournaments played with a specified lobby size
    /// </param>
    /// <param name="descending">Whether to sort the results in descending order</param>
    /// <remarks>None of the entities returned will be tracked by the context</remarks>
    Task<ICollection<Tournament>> GetAsync(
        int page,
        int pageSize,
        TournamentQuerySortType querySortType,
        bool verified = true,
        Ruleset? ruleset = null,
        string? searchQuery = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        VerificationStatus? verificationStatus = null,
        TournamentRejectionReason? rejectionReason = null,
        TournamentProcessingStatus? processingStatus = null,
        int? submittedBy = null,
        int? verifiedBy = null,
        int? lobbySize = null,
        bool descending = true
    );

    /// <summary>
    /// If the tournament is pre-rejected or pre-verified, updates the tournament
    /// to be rejected or verified respectively. This update strategy is applied
    /// to all child <see cref="Match"/>es, <see cref="Game"/>s, and
    /// <see cref="GameScore"/>s
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="verifierUserId">User id of the verifier</param>
    /// <returns>The updated <see cref="Tournament"/></returns>
    Task<Tournament?> AcceptPreVerificationStatusesAsync(int id, int verifierUserId);

    /// <summary>
    /// Resets the VerificationStatus, ProcessingStatus, WarningFlags (if applicable), and RejectionReasons for
    /// the tournament and all descendant entities
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="force">Whether to overwrite fully Verified/Rejected data (dangerous)</param>
    Task ResetAutomationStatusesAsync(int id, bool force = false);

    /// <summary>
    /// Gets the <see cref="Tournament"/>'s <see cref="Tournament.PooledBeatmaps"/> collection
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <returns>
    /// The <see cref="Tournament"/>'s <see cref="Tournament.PooledBeatmaps"/> collection or an
    /// empty collection if the tournament does not exist
    /// </returns>
    Task<ICollection<Beatmap>> GetPooledBeatmapsAsync(int id);

    /// <summary>
    /// Retrieves team size statistics for a specific player within a given ruleset and date range.
    /// The statistics are returned as a dictionary where the key represents the team size (number of players in a team)
    /// and the value represents the count of tournaments the player participated in with that team size.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player for whom the statistics are being retrieved.</param>
    /// <param name="ruleset">The ruleset (e.g., osu!, osu!taiko, osu!catch, osu!mania) to filter the tournaments.</param>
    /// <param name="dateMin">The lower bound of the date range to filter tournaments. Only tournaments occurring on or after this date will be included.</param>
    /// <param name="dateMax">The upper bound of the date range to filter tournaments. Only tournaments occurring on or before this date will be included.</param>
    /// <returns>
    /// A dictionary where each key is a team size (integer) and the corresponding value is the count of tournaments
    /// the player participated in with that team size. If no tournaments match the criteria, the keys are populated with a value of 0.
    /// </returns>
    /// <remarks>
    /// Lookup -1 for "Other" format statistics
    /// </remarks>
    Task<Dictionary<int, int>> GetLobbySizeStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax
    );

    /// <summary>
    /// Search for a tournament by name or acronym
    /// </summary>
    /// <param name="name">Tournament name or acronym</param>
    public Task<IList<Tournament>> SearchAsync(string name);

    /// <summary>
    /// Adds a collection of osu! beatmap ids to the <see cref="Tournament"/>'s <see cref="Tournament.PooledBeatmaps"/>
    /// collection
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="osuBeatmapIds">A collection of osu! beatmap ids to add</param>
    /// <returns>
    /// The <see cref="Tournament"/>'s <see cref="Tournament.PooledBeatmaps"/> collection or an
    /// empty collection if the tournament does not exist
    /// </returns>
    Task<ICollection<Beatmap>> AddPooledBeatmapsAsync(int id, ICollection<long> osuBeatmapIds);

    /// <summary>
    /// Unmaps all pooled beatmaps from a given tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    Task DeletePooledBeatmapsAsync(int id);

    /// <summary>
    /// Unmaps the provided beatmap ids from being pooled in the given tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="beatmapIds">Collection of beatmap ids to remove from the tournament's collection of pooled beatmaps</param>
    Task DeletePooledBeatmapsAsync(int id, ICollection<int> beatmapIds);

    /// <summary>
    /// Gets a map of <see cref="Common.Enums.Verification.VerificationStatus"/>es to the number of <see cref="Database.Entities.Tournament"/>s with the status
    /// </summary>
    Task<Dictionary<VerificationStatus, int>> GetVerificationStatusStatsAsync();

    /// <summary>
    /// Gets a map of years to the number of <see cref="Database.Entities.Tournament"/>s in that year
    /// </summary>
    /// <param name="verified">Filters results for only verified tournaments</param>
    Task<Dictionary<int, int>> GetYearStatsAsync(bool verified = true);

    /// <summary>
    /// Gets a map of <see cref="Common.Enums.Ruleset"/>s to the number of <see cref="Database.Entities.Tournament"/>s in that ruleset
    /// </summary>
    /// <param name="verified">Filters results for only verified tournaments</param>
    Task<Dictionary<Ruleset, int>> GetRulesetStatsAsync(bool verified = true);

    /// <summary>
    /// Gets a map of lobby sizes to the number of <see cref="Database.Entities.Tournament"/>s with that lobby size
    /// </summary>
    /// <param name="verified">Filters results for only verified tournaments</param>
    Task<Dictionary<int, int>> GetLobbySizeStatsAsync(bool verified = true);

    /// <summary>
    /// Loads the matches, games, and scores for a tournament to enable cascading operations
    /// </summary>
    /// <param name="tournament">The tournament to load children for</param>
    Task LoadMatchesWithGamesAndScoresAsync(Tournament tournament);
}
