using Database.Enums;
using OsuApiClient.Domain.Beatmaps;
using OsuApiClient.Domain.Multiplayer;
using OsuApiClient.Domain.Users;
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
    /// By default the endpoint returns a match with only the last 100 events that occurred.
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
    /// <param name="beatmapId">Id of the beatmap</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="BeatmapExtended"/>, or null if the request was unsuccessful</returns>
    Task<BeatmapExtended?> GetBeatmapAsync(
        long beatmapId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Clears the current access credentials for the client
    /// </summary>
    void ClearCredentials();
}
