using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs;
using API.Entities;
using API.Handlers.Interfaces;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using OsuSharp.Interfaces;

namespace API.Handlers.Implementations;

/// <summary>
/// Handles API access from clients
/// </summary>
public class OAuthHandler : IOAuthHandler
{
    private readonly ILogger<OAuthHandler> _logger;
    private readonly IOAuthClientService _clientService;
    private readonly IOsuClient _osuClient;
    private readonly IConfiguration _configuration;
    private readonly IPlayerRepository _playerRepository;
    private readonly IUserRepository _userRepository;

    private readonly string _jwtAudience;
    private readonly string _jwtKey;

    private const int ACCESS_DURATION_SECONDS = 60;
    
    public OAuthHandler(ILogger<OAuthHandler> logger, IOAuthClientService clientService, 
        IOsuClient osuClient, IConfiguration configuration, IPlayerRepository playerRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _clientService = clientService;
        _osuClient = osuClient;
        _configuration = configuration;
        _playerRepository = playerRepository;
        _userRepository = userRepository;

        _jwtAudience = _configuration["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer missing from config");
        _jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key missing from config");
    }

    public async Task<OAuthResponseDTO?> AuthorizeAsync(string osuAuthCode)
    {
        _logger.LogDebug("Attempting authorization via osuAuthToken");

        if (string.IsNullOrEmpty(osuAuthCode))
        {
            _logger.LogDebug("osuAuthToken null or empty, cannot authorize");
            return null;
        }

        var osuUser = await AuthorizeOsuUserAsync(osuAuthCode);
        var player = await _playerRepository.GetOrCreateAsync(osuUser.Id);
        var user = await AuthenticateUserAsync(player.Id);
        
        // Because this is a user, we need to also encode a user permission.
        // This is an alternative to storing it in a database (meaning,
        // no redundant "user" role for users).
        user.Scopes = user.Scopes.Append("user").ToArray();
        
        // Because this user is logging in with osu!, we can
        // issue a new refresh token.
        var accessToken = GenerateAccessToken(user.Id.ToString(), _jwtAudience, user.Scopes, ACCESS_DURATION_SECONDS);
        var refreshToken = GenerateRefreshToken(user.Id.ToString(), _jwtAudience);

        _logger.LogDebug("Authorized user with id {Id}, access expires in {seconds}", user.Id, ACCESS_DURATION_SECONDS);
        
        return new OAuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessExpiration = ACCESS_DURATION_SECONDS
        };
    }

    public async Task<OAuthResponseDTO?> AuthorizeAsync(int clientId, string clientSecret)
    {
        bool validClient = await _clientService.ValidateAsync(clientId, clientSecret);
        if (!validClient)
        {
            return null;
        }

        var client = await _clientService.GetAsync(clientId);
        if (client == null)
        {
            // Very unlikely, but possible
            return null;
        }
        
        // Add the 'client' scope to the scopes array
        client.Scopes = client.Scopes.Append("client").ToArray();
        
        return new OAuthResponseDTO
        {
            AccessToken = GenerateAccessToken(clientId.ToString(), _jwtAudience, client.Scopes, ACCESS_DURATION_SECONDS),
            RefreshToken = GenerateRefreshToken(clientId.ToString(), _jwtAudience),
            AccessExpiration = ACCESS_DURATION_SECONDS
        };
    }

    public async Task<OAuthResponseDTO?> RefreshAsync(string accessToken, string refreshToken)
    {
        var decryptedAccess = ReadToken(accessToken);
        var decryptedRefresh = ReadToken(refreshToken);

        var accessIssuer = decryptedAccess.Issuer;
        var refreshIssuer = decryptedRefresh.Issuer;

        if (accessIssuer != refreshIssuer)
        {
            return null;
        }

        // The ids of the access token and refresh token align. Validate the user's information
        if (!int.TryParse(accessIssuer, out int issuerId))
        {
            _logger.LogWarning("Failed to decrypt access token issuer into an integer (id)");
            return null;
        }
        
        // Validate the identity of the issuer
        var user = await _userRepository.GetAsync(issuerId);
        if (user == null)
        {
            _logger.LogWarning("Decrypted access token issuer is not a valid user");
            return null;
        }
        
        // Return a new OAuthResponseDTO containing only a new access token, NOT a new refresh token.
        return new OAuthResponseDTO
        {
            AccessToken = GenerateAccessToken(accessIssuer, _jwtAudience, user.Scopes, ACCESS_DURATION_SECONDS),
            RefreshToken = refreshToken,
            AccessExpiration = ACCESS_DURATION_SECONDS
        };
    }

    public async Task<OAuthClientDTO?> CreateClientAsync(int userId, params string[] scopes)
    {
        var secret = GenerateClientSecret();

        while (await _clientService.SecretInUse(secret))
        {
            secret = GenerateClientSecret();
        }
        
        return await _clientService.CreateAsync(userId, secret, scopes);
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given user id and set of roles (claims).
    /// Encodes the identity, claims, and expiration into the JWT. This JWT acts as
    /// the OAuth2 access token.
    /// </summary>
    /// <param name="issuer">The id of the user or client who generated this token</param>
    /// <param name="audience">The intended recipient of the JWT. This should be an environment variable.</param>
    /// <param name="roles">The claims the user has</param>
    /// <param name="expirationSeconds">Token expiration</param>
    /// <returns></returns>
    private string GenerateAccessToken(string issuer, string audience, string[] roles, int expirationSeconds)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role));
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
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static JwtSecurityToken ReadToken(string token)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(token);
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for a given user id to act as an OAuth2 refresh token.
    /// </summary>
    /// <param name="issuer">The id of the user or client who generated this token</param>
    /// <param name="audience">The issuer of the JWT. This should be an environment variable.</param>
    /// <param name="expirationSeconds">The expiration in seconds</param>
    /// <returns></returns>
    private string GenerateRefreshToken(string issuer, string audience, int expirationSeconds = 1_209_600)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer, audience,
            expires: DateTime.UtcNow.AddSeconds(expirationSeconds),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
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
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[r.Next(s.Length)]).ToArray());
    }
    
    private async Task<IGlobalUser> AuthorizeOsuUserAsync(string osuCode)
    {
        string cbUrl = _configuration["Auth:ClientCallbackUrl"] ?? throw new Exception("Missing Auth:ClientCallbackUrl in configuration!!");
        // Use OsuSharp to validate that the user is who they say they are
        await _osuClient.GetAccessTokenFromCodeAsync(osuCode, cbUrl);
        return await _osuClient.GetCurrentUserAsync();
    }
    
    private async Task<User> AuthenticateUserAsync(int playerId)
    {
        // Double db call, kind of inefficient
        var user = await _userRepository.GetOrCreateAsync(playerId);

        user.LastLogin = DateTime.UtcNow;
        user.Updated = DateTime.UtcNow;
        user.PlayerId = playerId;
        
        await _userRepository.UpdateAsync(user);
        return user;
    }
}