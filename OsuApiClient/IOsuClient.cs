using Database.Enums;
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
    /// The current credentials if they are not revoked or expired,
    /// the new credentials,
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
    /// Access credentials for the user, or null if the request was unsuccessful
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
    /// Access credentials for the user, or null if the request was unsuccessful
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
    /// <returns>The user, or null if there is no currently authenticated user or the request was unsuccessful</returns>
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
    /// <returns>A user, or null if the request was unsuccessful</returns>
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
    /// <returns>A user, or null if the request was unsuccessful</returns>
    Task<UserExtended?> GetUserAsync(
        string username,
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Clears the current access credentials for the client
    /// </summary>
    void ClearCredentials();
}
