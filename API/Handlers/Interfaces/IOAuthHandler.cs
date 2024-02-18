using API.Entities;

namespace API.Handlers.Interfaces;

public interface IOAuthHandler
{
    /// <summary>
    /// Authorizes a new or returning user via osu!
    /// </summary>
    /// <param name="osuAuthToken">The token provided by
    /// the osu! oAuth redirect.
    /// 
    /// <a href="https://osu.ppy.sh/docs/index.html#authorization-code-grant">
    /// osu! Authorization Code Grant documentation</a>
    /// </param>
    Task<User?> AuthorizeAsync(string osuAuthToken);

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
    Task<string> GenerateAccessToken(int userId, string issuer, string[] roles, int expirationSeconds = 3600);

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given user id to act as an OAuth2 refresh token.
    /// </summary>
    /// <param name="userId">The id of the user the token belongs to</param>
    /// <param name="issuer">The issuer of the JWT. This should be an environment variable.</param>
    /// <param name="expirationSeconds">The expiration in seconds</param>
    /// <returns></returns>
    Task<string> GenerateRefreshToken(int userId, string issuer, int expirationSeconds = 1_209_600);
}