using API.DTOs;
using API.Handlers.Interfaces;

namespace API.Handlers.Implementations;

/// <summary>
/// Handles API access from clients
/// </summary>
public class OAuthHandler : IOAuthHandler
{
    public Task<OAuthResponseDTO?> AuthorizeAsync(string osuAuthToken)
    {
        throw new NotImplementedException();
    }

    public Task<OAuthResponseDTO?> AuthorizeAsync(int userId, int clientId, string clientSecret)
    {
        throw new NotImplementedException();
    }

    public Task<OAuthResponseDTO?> RefreshAsync(string accessToken, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<OAuthClientDTO> CreateClientAsync(int userId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given user id and set of roles (claims).
    /// Encodes the identity, claims, and expiration into the JWT. This JWT acts as
    /// the OAuth2 access token.
    /// </summary>
    /// <param name="userId">The id of the user who has access to this token</param>
    /// <param name="issuer">The issuer of the JWT. This should be an environment variable.</param>
    /// <param name="roles">The claims the user has</param>
    /// <param name="expirationSeconds">Token expiration</param>
    /// <returns></returns>
    private string GenerateAccessToken(int userId, string issuer, string[] roles, int expirationSeconds = 3600)
    {
        return string.Empty;
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given user id to act as an OAuth2 refresh token.
    /// </summary>
    /// <param name="userId">The id of the user the token belongs to</param>
    /// <param name="issuer">The issuer of the JWT. This should be an environment variable.</param>
    /// <param name="expirationSeconds">The expiration in seconds</param>
    /// <returns></returns>
    private string GenerateRefreshToken(int userId, string issuer, int expirationSeconds = 1_209_600)
    {
        return string.Empty;
    }
}