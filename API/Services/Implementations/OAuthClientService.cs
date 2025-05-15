using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Authorization;
using API.Configurations;
using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services.Implementations;

public class OAuthClientService(
    ILogger<OAuthClientService> logger,
    IOAuthClientRepository oAuthClientRepository,
    IPasswordHasher<OAuthClient> passwordHasher,
    IOptions<JwtConfiguration> jwtConfiguration,
    IMapper mapper
    ) : IOAuthClientService
{

    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    // Default: 1 hour
    private const int AccessDurationSeconds = 3600;

    public async Task<bool> ExistsAsync(int id, int userId) =>
        await oAuthClientRepository.ExistsAsync(id, userId);

    public async Task<OAuthClientDTO?> GetAsync(int id) =>
        mapper.Map<OAuthClientDTO?>(await oAuthClientRepository.GetAsync(id));

    public async Task<OAuthClientCreatedDTO> CreateAsync(int userId, params string[] scopes)
    {
        var secret = oAuthClientRepository.GenerateClientSecret();
        var client = new OAuthClient
        {
            Scopes = scopes,
            Secret = secret,
            UserId = userId
        };

        OAuthClient newClient = await oAuthClientRepository.CreateAsync(client);

        OAuthClientCreatedDTO dto = mapper.Map<OAuthClientCreatedDTO>(newClient);
        dto.ClientSecret = secret;

        return dto;
    }

    public async Task<bool> DeleteAsync(int id) =>
        (await oAuthClientRepository.DeleteAsync(id)).HasValue;

    public async Task<OAuthClientDTO?> SetRateLimitOverrideAsync(int id, int rateLimitOverride) =>
        mapper.Map<OAuthClientDTO>(await oAuthClientRepository.SetRateLimitOverrideAsync(id, rateLimitOverride));

    public async Task<OAuthClientCreatedDTO?> ResetSecretAsync(int id)
    {
        OAuthClient? client = await oAuthClientRepository.GetAsync(id);
        if (client is null)
        {
            return null;
        }

        var newSecret = oAuthClientRepository.GenerateClientSecret();
        var hashedSecret = passwordHasher.HashPassword(client, newSecret);

        client.Secret = hashedSecret;
        await oAuthClientRepository.UpdateAsync(client);

        OAuthClientCreatedDTO dto = mapper.Map<OAuthClientCreatedDTO>(client);
        dto.ClientSecret = newSecret;

        return dto;
    }

    public async Task<AccessCredentialsDTO?> AuthenticateAsync(int clientId, string clientSecret)
    {
        OAuthClient? client = await oAuthClientRepository.GetAsync(clientId);
        if (client is null)
        {
            return null;
        }

        // Validate secret
        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(client, client.Secret, clientSecret);
        if (result != PasswordVerificationResult.Success)
        {
            return null;
        }

        logger.LogDebug("Authenticated client with id {Id}", clientId);

        return new AccessCredentialsDTO
        {
            AccessToken = CreateAccessToken(client),
            ExpiresIn = AccessDurationSeconds
        };
    }

    /// <summary>
    /// Creates a JWT access token for the given client
    /// </summary>
    /// <param name="client">Client</param>
    private string CreateAccessToken(OAuthClient client)
    {
        List<Claim> claims =
        [
            new(OtrClaims.Role, OtrClaims.Roles.Client),
            new(OtrClaims.Subject, client.Id.ToString()),
            new(OtrClaims.Instance, Guid.NewGuid().ToString()),
            ..client.Scopes.Select(s => new Claim(OtrClaims.Role, s))
        ];

        if (client.RateLimitOverride.HasValue)
        {
            claims.Add(new Claim(OtrClaims.RateLimitOverrides, client.RateLimitOverride.Value.ToString()));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(AccessDurationSeconds),
            Audience = _jwtConfiguration.Audience,
            Issuer = _jwtConfiguration.Issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key)),
                SecurityAlgorithms.HmacSha256
            ),
        }));
    }
}
