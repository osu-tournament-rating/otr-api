using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Configurations;
using API.DTOs;
using API.Entities;
using API.Handlers.Interfaces;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OsuSharp.Interfaces;

namespace API.Handlers.Implementations;

/// <summary>
/// Handles API access from clients
/// </summary>
public class OAuthHandler(
    ILogger<OAuthHandler> logger,
    IOAuthClientService clientService,
    IOAuthClientRepository clientRepository,
    IOsuClient osuClient,
    IOptions<JwtConfiguration> jwtConfiguration,
    IOptions<AuthConfiguration> authConfiguration,
    IPlayerRepository playerRepository,
    IUserRepository userRepository
    ) : IOAuthHandler
{
    private const int AccessDurationSeconds = 3600;
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
            RefreshToken = GenerateRefreshToken(user.Id.ToString(), "user"),
            AccessExpiration = AccessDurationSeconds
        };
    }

    public async Task<OAuthResponseDTO?> AuthorizeAsync(int clientId, string clientSecret)
    {
        logger.LogDebug("Attempting authorization via client credentials");

        // Get the client and validate secret
        OAuthClient? client = await clientRepository.GetAsync(clientId);
        if (client is null || client.Secret != clientSecret)
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
            RefreshToken = GenerateRefreshToken(clientId.ToString(), "client"),
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

    public async Task<OAuthClientDTO> CreateClientAsync(int userId, params string[] scopes)
    {
        var secret = GenerateClientSecret();

        while (await clientService.SecretInUse(secret))
        {
            secret = GenerateClientSecret();
        }

        return await clientService.CreateAsync(userId, secret, scopes);
    }

    /// <summary>
    /// Wrapper for generating a JSON Web Token (JWT) for a given client
    /// </summary>
    /// <remarks>Handles the encoding of rate limit overrides</remarks>
    private string GenerateAccessToken(OAuthClient client)
    {
        client.Scopes = [.. client.Scopes, "client"];
        IEnumerable<Claim> claims = client.Scopes.Select(role => new Claim(ClaimTypes.Role, role));
        var serializedOverrides = RateLimitOverridesSerializer.Serialize(client.RateLimitOverrides);
        if (!string.IsNullOrEmpty(serializedOverrides))
        {
            claims = [.. claims,
                new Claim(
                    OtrClaims.RateLimitOverrides,
                    serializedOverrides
                )
            ];
        }

        return GenerateAccessToken(
            client.Id.ToString(),
            claims
        );
    }

    /// <summary>
    /// Wrapper for generating a JSON Web Token (JWT) for a given user
    /// </summary>
    /// <remarks>Handles the encoding of rate limit overrides</remarks>
    private string GenerateAccessToken(User user)
    {
        user.Scopes = [.. user.Scopes, "user"];
        IEnumerable<Claim> claims = user.Scopes.Select(role => new Claim(ClaimTypes.Role, role));
        var serializedOverrides = RateLimitOverridesSerializer.Serialize(user.RateLimitOverrides);
        if (!string.IsNullOrEmpty(serializedOverrides))
        {
            claims = [.. claims,
                new Claim(
                OtrClaims.RateLimitOverrides,
                serializedOverrides
                )
            ];
        }

        return GenerateAccessToken(
            user.Id.ToString(),
            claims
        );
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given issuer and set of roles (claims) to act as the OAuth2 access token.
    /// </summary>
    /// <param name="issuer">The id of the user or client this token is generated for</param>
    /// <param name="claims">The claims used to form the <see cref="ClaimsIdentity"/></param>
    private string GenerateAccessToken(string issuer, IEnumerable<Claim> claims)
    {
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
    /// Deserializes a string into a <see cref="JwtSecurityToken"/>
    /// </summary>
    private static JwtSecurityToken ReadToken(string token) =>
        new JwtSecurityTokenHandler().ReadJwtToken(token);

    /// <summary>
    /// Serializes a token descriptor into a string
    /// </summary>
    /// <remarks>Adds a unique GUID to each token to ensure randomness</remarks>
    private string WriteToken(SecurityTokenDescriptor tokenDescriptor)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        tokenDescriptor.SigningCredentials = credentials;
        tokenDescriptor.Audience = jwtConfiguration.Value.Audience;
        tokenDescriptor.Claims.Add("instance", Guid.NewGuid().ToString());

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given issuer to act as an OAuth2 refresh token.
    /// </summary>
    /// <param name="issuer">The id of the user or client this token is generated for</param>
    /// <param name="role">
    /// The role value to encode to the JWT. This value needs to be either <see cref="OtrClaims.User"/>
    /// or <see cref="OtrClaims.Client"/> depending on what type of entity this token is generated for
    /// </param>
    /// <returns>A new refresh token</returns>
    private string GenerateRefreshToken(string issuer, string role)
    {
        if (role != "user" && role != "client")
        {
            throw new ArgumentException("Role must be either 'user' or 'client'");
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Role, role)]),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(RefreshDurationSeconds),
            Issuer = issuer
        };

        return WriteToken(tokenDescriptor);
    }

    /// <summary>
    /// Generates a new alpha-numeric client secret (size 50 chars)
    /// </summary>
    private static string GenerateClientSecret()
    {
        const int length = 50;
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var r = new Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[r.Next(s.Length)]).ToArray());
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
