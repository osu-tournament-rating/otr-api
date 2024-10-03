using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Authorization;
using API.Configurations;
using API.Services.Interfaces;
using Database.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services.Implementations;

public class JwtService(
    ILogger<JwtService> logger,
    IOptions<JwtConfiguration> jwtConfiguration
) : IJwtService
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    // Default: 1 hour
    public int AccessDurationSeconds => 3600;

    // Default: 2 weeks
    public int RefreshDurationSeconds => 1_209_600;

    public string GenerateAccessToken(OAuthClient client) =>
        GenerateAccessToken(
            client.Id.ToString(),
            [.. client.Scopes, OtrClaims.Roles.Client],
            client.RateLimitOverrides
        );

    public string GenerateAccessToken(User user) =>
        GenerateAccessToken(
            user.Id.ToString(),
            [.. user.Scopes, OtrClaims.Roles.User],
            user.RateLimitOverrides
        );

    public string GenerateRefreshToken(OAuthClient client) =>
        GenerateRefreshToken(client.Id.ToString(), OtrClaims.Roles.User);

    public string GenerateRefreshToken(User user) =>
        GenerateRefreshToken(user.Id.ToString(), OtrClaims.Roles.User);

    public ClaimsPrincipal? ReadToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        TokenValidationParameters validationParameters = DefaultTokenValidationParameters.Get(
            _jwtConfiguration.Issuer,
            _jwtConfiguration.Key,
            _jwtConfiguration.Audience
        );

        ClaimsPrincipal? result = null;
        try
        {
            result = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken _);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Token could not be validated {ex}", ex);
        }

        return result?.Identity is { IsAuthenticated: true } ? result : null;
    }

    /// <summary>
    /// Generates an access token
    /// </summary>
    /// <param name="subject">Id of the subject as a string</param>
    /// <param name="roles">Any <see cref="OtrClaims.Roles"/> the subject belongs to</param>
    /// <param name="rateLimitOverrides">Any <see cref="RateLimitOverrides"/>, if applicable</param>
    private string GenerateAccessToken(
        string subject,
        IEnumerable<string> roles,
        RateLimitOverrides? rateLimitOverrides = null
    )
    {
        var claims = new List<Claim>
        {
            new(OtrClaims.TokenType, OtrClaims.TokenTypes.AccessToken),
            new(OtrClaims.Subject, subject)
        };

        // Encode roles
        claims.AddRange(roles.Select(r => new Claim(OtrClaims.Role, r)));

        // Handle rate limit overrides
        if (rateLimitOverrides is not null)
        {
            var serializedOverrides = RateLimitOverridesSerializer.Serialize(rateLimitOverrides);
            if (!string.IsNullOrEmpty(serializedOverrides))
            {
                claims.Add(new Claim(OtrClaims.RateLimitOverrides, serializedOverrides));
            }
        }

        return WriteToken(claims, AccessDurationSeconds);
    }

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <param name="subject">Id of the subject as a string</param>
    /// <param name="role">
    /// The primary <see cref="OtrClaims.Role"/> of the subject.
    /// Must be of either <see cref="OtrClaims.Roles.User"/> or <see cref="OtrClaims.Roles.Client"/>
    /// </param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="role"/> is not of
    /// either <see cref="OtrClaims.Roles.User"/> or <see cref="OtrClaims.Roles.Client"/>
    /// </exception>
    private string GenerateRefreshToken(string subject, string role)
    {
        if (role is not OtrClaims.Roles.User && role is not OtrClaims.Roles.Client)
        {
            throw new ArgumentException(
                $"Role must be of either ${OtrClaims.Roles.User} or ${OtrClaims.Roles.Client}"
            );
        }

        return WriteToken(
            [
                new Claim(OtrClaims.TokenType, OtrClaims.TokenTypes.RefreshToken),
                new Claim(OtrClaims.Subject, subject),
                new Claim(OtrClaims.Role, role)
            ],
            RefreshDurationSeconds
        );
    }

    /// <summary>
    /// Writes a new JWT
    /// </summary>
    /// <param name="claims">Claims</param>
    /// <param name="expiry">Lifetime of the token in seconds</param>
    /// <returns>A new JWT encoded as a string</returns>
    private string WriteToken(IEnumerable<Claim> claims, int expiry)
    {
        // Encode instance
        claims = [.. claims, new Claim(OtrClaims.Instance, Guid.NewGuid().ToString())];

        // Create and write the token
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(expiry),
            Audience = _jwtConfiguration.Audience,
            Issuer = _jwtConfiguration.Issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key)),
                SecurityAlgorithms.HmacSha256
            ),
        }));
    }
}
