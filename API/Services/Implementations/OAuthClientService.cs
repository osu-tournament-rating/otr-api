using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Services.Implementations;

public class OAuthClientService(
    IOAuthClientRepository oAuthClientRepository,
    IPasswordHasher<OAuthClient> passwordHasher,
    IMapper mapper
    ) : IOAuthClientService
{
    public async Task<bool> ExistsAsync(int id, int userId) =>
        await oAuthClientRepository.ExistsAsync(id, userId);

    public async Task<OAuthClientDTO?> GetAsync(int id) =>
        mapper.Map<OAuthClientDTO?>(await oAuthClientRepository.GetAsync(id));

    public async Task<OAuthClientCreatedDTO> CreateAsync(int userId, params string[] scopes)
    {
        var secret = oAuthClientRepository.GenerateClientSecret();
        var client = new OAuthClient { Scopes = scopes, Secret = secret, UserId = userId };

        OAuthClient newClient = await oAuthClientRepository.CreateAsync(client);

        OAuthClientCreatedDTO dto = mapper.Map<OAuthClientCreatedDTO>(newClient);
        dto.ClientSecret = secret;

        return dto;
    }

    public async Task<bool> DeleteAsync(int id) =>
        (await oAuthClientRepository.DeleteAsync(id)).HasValue;

    public async Task<OAuthClientDTO?> SetRatelimitOverridesAsync(int id, RateLimitOverrides rateLimitOverrides) =>
        mapper.Map<OAuthClientDTO>(await oAuthClientRepository.SetRatelimitOverridesAsync(id, rateLimitOverrides));

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
}
