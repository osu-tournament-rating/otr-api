using System.Security.Claims;
using Database.Entities;

namespace API.Services.Interfaces;

public interface IJwtService
{
    /// <summary>
    /// Lifetime of an access token in seconds
    /// </summary>
    public int AccessDurationSeconds { get; }

    /// <summary>
    /// Lifetime of a refresh token in seconds
    /// </summary>
    public int RefreshDurationSeconds { get; }

    /// <summary>
    /// Generates an access token for an <see cref="OAuthClient"/>
    /// </summary>
    public string GenerateAccessToken(OAuthClient client);

    /// <summary>
    /// Generates an access token for a <see cref="User"/>
    /// </summary>
    public string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a refresh token for a <see cref="OAuthClient"/>
    /// </summary>
    public string GenerateRefreshToken(OAuthClient client);

    /// <summary>
    /// Generates a refresh token for a <see cref="User"/>
    /// </summary>
    public string GenerateRefreshToken(User user);

    /// <summary>
    /// Attempts to read and validate an encoded JWT
    /// </summary>
    /// <param name="token">Encoded JWT</param>
    /// <returns>The <see cref="ClaimsPrincipal"/> describing the JWT if successful</returns>
    public ClaimsPrincipal? ReadToken(string token);
}
