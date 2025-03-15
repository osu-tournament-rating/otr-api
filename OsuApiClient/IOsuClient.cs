using Common.Enums;
using OsuApiClient.Domain.Osu.Beatmaps;
using OsuApiClient.Domain.Osu.Multiplayer;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Domain.OsuTrack;
using OsuApiClient.Net.Authorization;

namespace OsuApiClient;

/// <summary>
/// Interfaces a client that communicates with the osu! API
/// </summary>
public interface IOsuClient : IDisposable
{
    /// <summary>
    /// Updates the current access credentials
    /// </summary>
    /// <remarks>
    /// If no refresh token is present in the current credentials, tries to get credentials for the client.
    /// See <a href="https://osu.ppy.sh/docs/index.html#client-credentials-grant">Client Credentials Grant</a>
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// The current <see cref="OsuCredentials"/> if they are not revoked or expired,
    /// the new <see cref="OsuCredentials"/>,
    /// or null if update was unsuccessful
    /// </returns>
    Task<OsuCredentials?> UpdateCredentialsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets and stores access credentials for a user by exchanging an authorization code
    /// </summary>
    /// <remarks>
    /// See <a href="https://osu.ppy.sh/docs/index.html#authorization-code-grant">Authorization Code Grant</a>
    /// </remarks>
    /// <param name="authorizationCode">User authorization code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// The <see cref="OsuCredentials"/> for the user, or null if the request was unsuccessful
    /// </returns>
    Task<OsuCredentials?> AuthorizeUserWithCodeAsync(
        string authorizationCode,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets and stores access credentials for a user using an existing refresh token
    /// </summary>
    /// <remarks>
    /// See <a href="https://osu.ppy.sh/docs/index.html#authorization-code-grant">Authorization Code Grant</a>
    /// </remarks>
    /// <param name="refreshToken">User refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// The <see cref="OsuCredentials"/> for the user, or null if the request was unsuccessful
    /// </returns>
    Task<OsuCredentials?> AuthorizeUserWithTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the currently authenticated user
    /// </summary>
    /// <remarks>
    /// See <a href="https://osu.ppy.sh/docs/index.html#get-own-data">Get Own Data</a>
    /// </remarks>
    /// <param name="ruleset">Ruleset to query for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// The <see cref="UserExtended"/>, or null if there is no currently authenticated user or the request was unsuccessful
    /// </returns>
    Task<UserExtended?> GetCurrentUserAsync(
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a user
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/docs/index.html#get-user">Get User</a></remarks>
    /// <param name="id">Id of the user</param>
    /// <param name="ruleset">Ruleset to query for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="UserExtended"/>, or null if the request was unsuccessful</returns>
    Task<UserExtended?> GetUserAsync(
        long id,
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a user
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/docs/index.html#get-user">Get User</a></remarks>
    /// <param name="username">Username of the user</param>
    /// <param name="ruleset">Ruleset to query for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="UserExtended"/>, or null if the request was unsuccessful</returns>
    Task<UserExtended?> GetUserAsync(
        string username,
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a multiplayer match, limit 100 events
    /// </summary>
    /// <remarks>
    /// By default, the endpoint returns a match with only the last 100 events that occurred.
    /// For example, if you wanted the first 100 events, you would pass 0 for <paramref name="eventsAfterId"/>
    /// </remarks>
    /// <param name="matchId">Id of the multiplayer lobby</param>
    /// <param name="eventsBeforeId">Include only events before this id</param>
    /// <param name="eventsAfterId">Include only events after this id</param>
    /// <param name="eventsLimit">Max number of events to include. Clamped at a maximum of 100</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="MultiplayerMatch"/>, or null if the request was unsuccessful</returns>
    Task<MultiplayerMatch?> GetPartialMatchAsync(
        long matchId,
        long? eventsBeforeId = null,
        long? eventsAfterId = null,
        int? eventsLimit = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a complete multiplayer match
    /// </summary>
    /// <remarks>
    /// Will query the osu! API until it collects a complete list of match events
    /// </remarks>
    /// <param name="matchId">Id of the multiplayer lobby</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="MultiplayerMatch"/>, or null if the request was unsuccessful</returns>
    Task<MultiplayerMatch?> GetMatchAsync(
        long matchId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a beatmap
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/docs/index.html#get-beatmap">Get Beatmap</a></remarks>
    /// <param name="beatmapId">Id of the beatmap</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="BeatmapExtended"/>, or null if the request was unsuccessful</returns>
    Task<BeatmapExtended?> GetBeatmapAsync(
        long beatmapId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets multiple beatmaps
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/docs/index.html#get-beatmaps">Get Beatmaps</a></remarks>
    /// <param name="beatmapIds">Beatmap ids</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of <see cref="BeatmapExtended"/>, one for each provided id (if it exists)</returns>
    Task<IEnumerable<BeatmapExtended>?> GetBeatmapsAsync(
        ICollection<long> beatmapIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets difficulty attributes for a beatmap
    /// </summary>
    /// <remarks>
    /// See <a href="https://osu.ppy.sh/docs/index.html#get-beatmap-attributes">Get Beatmap Attributes</a>
    /// </remarks>
    /// <param name="beatmapId">Id of the beatmap</param>
    /// <param name="mods">Mod combination to request attributes for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="BeatmapAttributes"/>, or null if the request was unsuccessful</returns>
    Task<BeatmapAttributes?> GetBeatmapAttributesAsync(
        long beatmapId,
        Mods mods,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a beatmap set
    /// </summary>
    /// <remarks>
    /// See <a href="https://osu.ppy.sh/docs/index.html#get-apiv2beatmapsetsbeatmapset">Get Beatmapset</a>
    /// </remarks>
    /// <param name="beatmapsetId">Id of the beatmap set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="BeatmapsetExtended"/>, or null if the request was unsuccessful</returns>
    Task<BeatmapsetExtended?> GetBeatmapsetAsync(
        long beatmapsetId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets statistics updates for a user
    /// </summary>
    /// <remarks>
    /// See
    /// <a href="https://github.com/Ameobea/osutrack-api?tab=readme-ov-file#get-all-stats-updates-for-user">
    /// Get all stats updates for user
    /// </a>
    /// </remarks>
    /// <param name="id">Id of the user</param>
    /// <param name="ruleset">Ruleset to query for</param>
    /// <param name="fromDate">Include only updates after this date</param>
    /// <param name="toDate">Include only updates before this date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of <see cref="UserStatUpdate"/>, or null if the request was unsuccessful</returns>
    Task<IEnumerable<UserStatUpdate>?> GetUserStatsHistoryAsync(
        long id,
        Ruleset ruleset,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Clears the current access credentials for the client
    /// </summary>
    void ClearCredentials();
}
