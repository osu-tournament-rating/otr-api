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
    private readonly ILogger<OAuthHandler> _logger = logger;
    private readonly IOAuthClientService _clientService = clientService;
    private readonly IOAuthClientRepository _clientRepository = clientRepository;
    private readonly IOsuClient _osuClient = osuClient;
    private readonly IOptions<JwtConfiguration> _jwtConfiguration = jwtConfiguration;
    private readonly IOptions<AuthConfiguration> _authConfiguration = authConfiguration;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUserRepository _userRepository = userRepository;

    private const int ACCESS_DURATION_SECONDS = 3600;

    public async Task<OAuthResponseDTO?> AuthorizeAsync(string osuAuthCode)
    {
        _logger.LogDebug("Attempting authorization via osuAuthToken");

        if (string.IsNullOrEmpty(osuAuthCode))
        {
            _logger.LogDebug("osuAuthToken null or empty, cannot authorize");
            return null;
        }

        IGlobalUser osuUser = await AuthorizeOsuUserAsync(osuAuthCode);
        Player player = await _playerRepository.GetOrCreateAsync(osuUser.Id);
        User user = await AuthenticateUserAsync(player.Id);

        // Because this user is logging in with osu!, we can
        // issue a new refresh token.
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user.Id.ToString(), _jwtConfiguration.Value.Audience, "user");

        _logger.LogDebug(
            "Authorized user with id {Id}, access expires in {seconds}",
            user.Id,
            ACCESS_DURATION_SECONDS
        );

        return new OAuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessExpiration = ACCESS_DURATION_SECONDS
        };
    }

    public async Task<OAuthResponseDTO?> AuthorizeAsync(int clientId, string clientSecret)
    {
        var validClient = await _clientService.ValidateAsync(clientId, clientSecret);
        if (!validClient)
        {
            return null;
        }

        OAuthClient? client = await _clientRepository.GetAsync(clientId);
        if (client == null)
        {
            // Very unlikely, but possible
            return null;
        }

        return new OAuthResponseDTO
        {
            AccessToken = GenerateAccessToken(client),
            RefreshToken = GenerateRefreshToken(clientId.ToString(), _jwtConfiguration.Value.Audience, "client"),
            AccessExpiration = ACCESS_DURATION_SECONDS
        };
    }

    public async Task<OAuthResponseDTO?> RefreshAsync(string refreshToken)
    {
        JwtSecurityToken decryptedRefresh = ReadToken(refreshToken);

        if (decryptedRefresh.ValidTo < DateTime.UtcNow)
        {
            return null;
        }

        var refreshIssuer = decryptedRefresh.Issuer;

        // The ids of the access token and refresh token align. Validate the user's information
        if (!int.TryParse(refreshIssuer, out var issuerId))
        {
            _logger.LogWarning("Failed to decrypt refresh token issuer into an integer (id)");
            return null;
        }

        // Check the role of the issuer
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(decryptedRefresh.Claims));
        if (!claimsPrincipal.IsUser() && !claimsPrincipal.IsClient())
        {
            _logger.LogWarning("Refresh token does not have the user or client role.");
            return null;
        }

        if (claimsPrincipal.IsUser() && claimsPrincipal.IsClient())
        {
            _logger.LogError("Refresh token cannot have both user and client roles.");
            return null;
        }

        // If the issuer is a user, validate the user id.
        // If the issuer is a client, validate the client id.

        var accessToken = string.Empty;

        if (claimsPrincipal.IsUser())
        {
            User? user = await _userRepository.GetAsync(issuerId);
            if (user == null)
            {
                _logger.LogWarning("Decrypted refresh token issuer is not a valid user");
                return null;
            }
            accessToken = GenerateAccessToken(user);
        }
        else if (claimsPrincipal.IsClient())
        {
            OAuthClient? client = await _clientRepository.GetAsync(issuerId);
            if (client == null)
            {
                _logger.LogWarning("Decrypted refresh token issuer is not a valid client");
                return null;
            }
            accessToken = GenerateAccessToken(client);
        }

        // Return a new OAuthResponseDTO containing only a new access token, NOT a new refresh token.
        return new OAuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessExpiration = ACCESS_DURATION_SECONDS
        };
    }

    public async Task<OAuthClientDTO> CreateClientAsync(int userId, params string[] scopes)
    {
        var secret = GenerateClientSecret();

        while (await _clientService.SecretInUse(secret))
        {
            secret = GenerateClientSecret();
        }

        return await _clientService.CreateAsync(userId, secret, scopes);
    }

    /// <summary>
    /// Wrapper for generating a JSON Web Token (JWT) for a given client
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    private string GenerateAccessToken(OAuthClient client)
    {
        client.Scopes = [.. client.Scopes, "client"];
        IEnumerable<Claim> claims = client.Scopes.Select(role => new Claim(ClaimTypes.Role, role));
        if (client.RateLimitOverrides is not null)
        {
            claims = [.. claims,
                new Claim(
                OtrClaimTypes.RateLimitOverrides,
                RateLimitOverridesSerializer.Serialize(client.RateLimitOverrides))
            ];
        }

        return GenerateAccessToken(
            client.Id.ToString(),
            _jwtConfiguration.Value.Audience,
            claims,
            ACCESS_DURATION_SECONDS
        );
    }

    /// <summary>
    /// Wrapper for generating a JSON Web Token (JWT) for a given user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private string GenerateAccessToken(User user)
    {
        user.Scopes = [.. user.Scopes, "user"];
        IEnumerable<Claim> claims = user.Scopes.Select(role => new Claim(ClaimTypes.Role, role));
        if (user.RateLimitOverrides is not null)
        {
            claims = [.. claims,
                new Claim(
                OtrClaimTypes.RateLimitOverrides,
                RateLimitOverridesSerializer.Serialize(user.RateLimitOverrides))
            ];
        }

        return GenerateAccessToken(
            user.Id.ToString(),
            _jwtConfiguration.Value.Audience,
            claims,
            ACCESS_DURATION_SECONDS
        );
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given user id and set of roles (claims).
    /// Encodes the identity, claims, and expiration into the JWT. This JWT acts as
    /// the OAuth2 access token.
    /// </summary>
    /// <param name="issuer">The id of the user or client who generated this token</param>
    /// <param name="audience">The intended recipient of the JWT. This should be an environment variable.</param>
    /// <param name="claims">The claims used to form the <see cref="ClaimsIdentity"/></param>
    /// <param name="expiration">Token expiration in seconds</param>
    /// <returns></returns>
    private string GenerateAccessToken(string issuer, string audience, IEnumerable<Claim> claims, int expiration)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        // Add randomness to each token with a unique GUID
        claims = [.. claims, new Claim("instance", Guid.NewGuid().ToString())];
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(expiration),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static JwtSecurityToken ReadToken(string token)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(token);
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given issuer to act as an OAuth2 refresh token.
    /// </summary>
    /// <param name="issuer">The id of the user or client who generated this token</param>
    /// <param name="audience">The issuer of the JWT. This should be an environment variable.</param>
    /// <param name="role">The role that applies to the JWT. For a refresh token, this should only be 'user' or 'client'
    /// depending on what type of entity generated this token.</param>
    /// <param name="expirationSeconds">The expiration in seconds</param>
    /// <returns></returns>
    private string GenerateRefreshToken(
        string issuer,
        string audience,
        string role,
        int expirationSeconds = 1_209_600
    )
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        if (role != "user" && role != "client")
        {
            throw new ArgumentException("Role must be either 'user' or 'client'");
        }

        Claim[] claims = [new Claim(ClaimTypes.Role, role), new Claim("instance", Guid.NewGuid().ToString())];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(expirationSeconds),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates a new alpha-numeric client secret (size 50 chars)
    /// </summary>
    /// <returns></returns>
    private static string GenerateClientSecret()
    {
        const int length = 50;
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var r = new Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[r.Next(s.Length)]).ToArray());
    }

    private async Task<IGlobalUser> AuthorizeOsuUserAsync(string osuCode)
    {
        // Use OsuSharp to authorize via osu! API v2
        await _osuClient.GetAccessTokenFromCodeAsync(osuCode, _authConfiguration.Value.ClientCallbackUrl);
        return await _osuClient.GetCurrentUserAsync();
    }

    private async Task<User> AuthenticateUserAsync(int playerId)
    {
        // Double db call, kind of inefficient
        User user = await _userRepository.GetOrCreateAsync(playerId);

        user.LastLogin = DateTime.UtcNow;
        user.Updated = DateTime.UtcNow;
        user.PlayerId = playerId;

        await _userRepository.UpdateAsync(user);
        return user;
    }
}
