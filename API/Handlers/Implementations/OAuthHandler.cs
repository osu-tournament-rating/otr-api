using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Authorization;
using API.Configurations;
using API.DTOs;
using API.Handlers.Interfaces;
using API.Repositories.Interfaces;
using API.Utilities.Extensions;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OsuSharp.Interfaces;

namespace API.Handlers.Implementations;

/// <summary>
/// Handles reading and serving access credentials
/// </summary>
public class OAuthHandler(
    ILogger<OAuthHandler> logger,
    IOAuthClientRepository clientRepository,
    IPlayersRepository playerRepository,
    IUserRepository userRepository,
    IOsuClient osuClient,
    IPasswordHasher<OAuthClient> clientSecretHasher,
    IOptions<JwtConfiguration> jwtConfiguration,
    IOptions<AuthConfiguration> authConfiguration
    ) : IOAuthHandler
{
    /// <summary>
    /// Lifetime of an access token in seconds
    /// </summary>
    private const int AccessDurationSeconds = 3600;

    /// <summary>
    /// Lifetime of a refresh token in seconds
    /// </summary>
    private const int RefreshDurationSeconds = 1_209_600;

    public async Task<OAuthResponseDTO?> AuthorizeAsync(string osuAuthCode)
    {
        logger.LogDebug("Attempting authorization via osuAuthToken");

        if (string.IsNullOrEmpty(osuAuthCode))
        {
            logger.LogDebug("osuAuthCode null or empty, cannot authorize");
            return null;
        }

        IGlobalUser osuUser = await AuthorizeOsuUserAsync(osuAuthCode);
        Player player = await playerRepository.GetOrCreateAsync(osuUser.Id);
        User user = await AuthenticateUserAsync(player.Id);

        logger.LogDebug(
            "Authorized user with id {Id}, access expires in {seconds}",
            user.Id,
            AccessDurationSeconds
        );

        return new OAuthResponseDTO
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = GenerateRefreshToken(user.Id.ToString(), OtrClaims.Roles.User),
            AccessExpiration = AccessDurationSeconds
        };
    }

    public async Task<OAuthResponseDTO?> AuthorizeAsync(int clientId, string clientSecret)
    {
        logger.LogDebug("Attempting authorization via client credentials");

        OAuthClient? client = await clientRepository.GetAsync(clientId);
        if (client is null)
        {
            return null;
        }

        // Validate secret
        PasswordVerificationResult result = clientSecretHasher.VerifyHashedPassword(client, client.Secret, clientSecret);
        if (result != PasswordVerificationResult.Success)
        {
            return null;
        }

        logger.LogDebug(
            "Authorized client with id {Id}, access expires in {seconds}",
            clientId,
            AccessDurationSeconds
        );

        return new OAuthResponseDTO
        {
            AccessToken = GenerateAccessToken(client),
            RefreshToken = GenerateRefreshToken(clientId.ToString(), OtrClaims.Roles.Client),
            AccessExpiration = AccessDurationSeconds
        };
    }

    public async Task<OAuthResponseDTO?> RefreshAsync(string refreshToken)
    {
        JwtSecurityToken decryptedRefresh = ReadToken(refreshToken);

        if (decryptedRefresh.ValidTo < DateTime.UtcNow)
        {
            return null;
        }

        if (!int.TryParse(decryptedRefresh.Issuer, out var issuerId))
        {
            logger.LogWarning("Failed to decrypt refresh token issuer into an integer (id)");
            return null;
        }

        // Validate the issuer is a user or client
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(decryptedRefresh.Claims));
        if (!claimsPrincipal.IsUser() && !claimsPrincipal.IsClient())
        {
            logger.LogWarning("Refresh token does not have the user or client role.");
            return null;
        }
        if (claimsPrincipal.IsUser() && claimsPrincipal.IsClient())
        {
            logger.LogError("Refresh token cannot have both user and client roles.");
            return null;
        }

        var accessToken = string.Empty;
        // Generate new access token
        if (claimsPrincipal.IsUser())
        {
            User? user = await userRepository.GetAsync(issuerId);
            if (user == null)
            {
                logger.LogWarning("Decrypted refresh token issuer is not a valid user");
                return null;
            }
            accessToken = GenerateAccessToken(user);
        }
        else if (claimsPrincipal.IsClient())
        {
            OAuthClient? client = await clientRepository.GetAsync(issuerId);
            if (client == null)
            {
                logger.LogWarning("Decrypted refresh token issuer is not a valid client");
                return null;
            }
            accessToken = GenerateAccessToken(client);
        }

        // Return a new OAuthResponseDTO containing only a new access token, NOT a new refresh token.
        return new OAuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessExpiration = AccessDurationSeconds
        };
    }

    /// <summary>
    /// Generates an access token for an <see cref="OAuthClient"/>
    /// </summary>
    private string GenerateAccessToken(OAuthClient client) =>
        GenerateAccessToken(
            client.Id.ToString(),
            [.. client.Scopes, OtrClaims.Roles.Client],
            client.RateLimitOverrides
        );

    /// <summary>
    /// Generates an access token for a <see cref="User"/>
    /// </summary>
    private string GenerateAccessToken(User user) =>
        GenerateAccessToken(
            user.Id.ToString(),
            [.. user.Scopes, OtrClaims.Roles.User],
            user.RateLimitOverrides
        );

    /// <summary>
    /// Generates an access token
    /// </summary>
    /// <param name="issuer">Id of the recipient as a string</param>
    /// <param name="roles">Any <see cref="OtrClaims.Roles"/> the recipient belongs to</param>
    /// <param name="rateLimitOverrides">Any <see cref="RateLimitOverrides"/>, if applicable</param>
    private string GenerateAccessToken(
        string issuer,
        IEnumerable<string> roles,
        RateLimitOverrides? rateLimitOverrides = null
    )
    {
        var claims = new List<Claim> { new(OtrClaims.TokenType, OtrClaims.TokenTypes.AccessToken) };

        // Encode roles
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Handle rate limit overrides
        if (rateLimitOverrides is not null)
        {
            var serializedOverrides = RateLimitOverridesSerializer.Serialize(rateLimitOverrides);
            if (!string.IsNullOrEmpty(serializedOverrides))
            {
                claims.Add(new Claim(OtrClaims.RateLimitOverrides, serializedOverrides));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(AccessDurationSeconds),
            Issuer = issuer
        };

        return WriteToken(tokenDescriptor);
    }

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <param name="issuer">Id of the recipient as a string</param>
    /// <param name="role">
    /// The <see cref="ClaimTypes.Role"/> of the recipient.
    /// Must be of either <see cref="OtrClaims.Roles.User"/> or <see cref="OtrClaims.Roles.Client"/>
    /// </param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="role"/> is not of
    /// either <see cref="OtrClaims.Roles.User"/> or <see cref="OtrClaims.Roles.Client"/>
    /// </exception>
    private string GenerateRefreshToken(string issuer, string role)
    {
        if (role is not OtrClaims.Roles.User && role is not OtrClaims.Roles.Client)
        {
            throw new ArgumentException(
                $"Role must be of either ${OtrClaims.Roles.User} or ${OtrClaims.Roles.Client}"
            );
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Role, role),
                new Claim(OtrClaims.TokenType, OtrClaims.TokenTypes.RefreshToken)
            ]),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(RefreshDurationSeconds),
            Issuer = issuer
        };

        return WriteToken(tokenDescriptor);
    }

    /// <summary>
    /// Deserializes a string into a <see cref="JwtSecurityToken"/>
    /// </summary>
    private static JwtSecurityToken ReadToken(string token) =>
        new JwtSecurityTokenHandler().ReadJwtToken(token);

    /// <summary>
    /// Creates, signs, and writes a <see cref="SecurityToken"/> into a string from a
    /// given <see cref="SecurityTokenDescriptor"/>
    /// </summary>
    /// <remarks>
    /// Handles encoding the <see cref="OtrClaims.Instance"/> and <see cref="SecurityTokenDescriptor.Audience"/> claims
    /// </remarks>
    private string WriteToken(SecurityTokenDescriptor tokenDescriptor)
    {
        // Assign signing credentials and audience
        tokenDescriptor.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Value.Key)),
            SecurityAlgorithms.HmacSha256
        );
        tokenDescriptor.Audience = jwtConfiguration.Value.Audience;

        // Encode instance
        tokenDescriptor.Claims.Add(OtrClaims.Instance, new Guid().ToString());

        // Create and write the token
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    /// <summary>
    /// Uses OsuSharp to authorize the current user via osu! API v2
    /// </summary>
    /// <param name="osuCode">The authorization code for the user</param>
    /// <returns>The authorized user</returns>
    private async Task<IGlobalUser> AuthorizeOsuUserAsync(string osuCode)
    {
        await osuClient.GetAccessTokenFromCodeAsync(osuCode, authConfiguration.Value.ClientCallbackUrl);
        return await osuClient.GetCurrentUserAsync();
    }

    /// <summary>
    /// Gets and "logs in" the user for the given player id
    /// </summary>
    private async Task<User> AuthenticateUserAsync(int playerId)
    {
        User user = await userRepository.GetByPlayerIdOrCreateAsync(playerId);

        user.LastLogin = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        return user;
    }
}
