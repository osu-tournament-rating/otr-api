using API.DTOs;
using API.Entities;
using API.Handlers.Interfaces;
using API.Services.Interfaces;

namespace API.Handlers.Implementations;

/// <summary>
/// Handles API access from clients
/// </summary>
public class OAuthHandler : IOAuthHandler
{
    private readonly IOAuthClientService _clientService;
    private readonly IConfiguration _configuration;

    public OAuthHandler(IOAuthClientService clientService, IConfiguration configuration)
    {
        _clientService = clientService;
        _configuration = configuration;
    }

    public async Task<OAuthResponseDTO?> AuthorizeAsync(string osuAuthToken)
    {
        throw new NotImplementedException();
    }

    public async Task<OAuthResponseDTO?> AuthorizeAsync(int userId, int clientId, string clientSecret)
    {
        throw new NotImplementedException();
    }

    public async Task<OAuthResponseDTO?> RefreshAsync(string accessToken, string refreshToken)
    {
        throw new NotImplementedException();
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
}