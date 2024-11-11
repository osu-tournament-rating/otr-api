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
    Task<AccessCredentialsDTO?> AuthorizeAsync(string osuAuthCode);

    /// <summary>
    /// Authorize an OAuth client via client credentials
    /// </summary>
    /// <param name="clientId">The id of the OAuth client</param>
    /// <param name="clientSecret">The client secret</param>
    /// <returns>Access credentials for the associated client, or null if there was a problem with authorization</returns>
    Task<DetailedResponseDTO<AccessCredentialsDTO>> AuthorizeAsync(int clientId, string clientSecret);

    /// <summary>
    /// Issues a new access token using the given refresh token
    /// </summary>
    /// <remarks>Will not generate a new refresh token</remarks>
    /// <returns>
    /// Access credentials containing a new access token, or null if the given refresh token is invalid
    /// </returns>
    Task<DetailedResponseDTO<AccessCredentialsDTO>> RefreshAsync(string refreshToken);
}
