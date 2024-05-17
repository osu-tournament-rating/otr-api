using API.DTOs;

namespace API.Handlers.Interfaces;

public interface IOAuthHandler
{
    /// <summary>
    /// Authorizes a new or returning user via osu!
    /// </summary>
    /// <param name="osuAuthCode">
    /// The token provided by the osu! oAuth redirect
    /// See <a href="https://osu.ppy.sh/docs/index.html#authorization-code-grant">
    /// osu! Authorization Code Grant documentation</a>
    /// </param>
    /// <returns>Access credentials for the associated user, or null if there was a problem with authorization</returns>
    Task<OAuthResponseDTO?> AuthorizeAsync(string osuAuthCode);

    /// <summary>
    /// Authorize an OAuth client via client credentials
    /// </summary>
    /// <param name="clientId">The id of the OAuth client</param>
    /// <param name="clientSecret">The client secret</param>
    /// <returns>Access credentials for the associated client, or null if there was a problem with authorization</returns>
    Task<OAuthResponseDTO?> AuthorizeAsync(int clientId, string clientSecret);

    /// <summary>
    /// Issues a new access token using the given refresh token
    /// </summary>
    /// <remarks>
    /// Will not generate a new refresh token. The given refresh token will be returned with the new access token
    /// </remarks>
    /// <returns>
    /// Access credentials containing a new access token, or null if the given refresh token is invalid
    /// </returns>
    Task<OAuthResponseDTO?> RefreshAsync(string refreshToken);

    /// <summary>
    /// Creates a new OAuth client for a user
    /// </summary>
    /// <param name="userId">The id of the user who owns this client</param>
    /// <param name="scopes">The scopes to assign to the client</param>
    /// <returns>Client credentials for the new client</returns>
    Task<OAuthClientCreatedDTO> CreateClientAsync(int userId, params string[] scopes);
}
