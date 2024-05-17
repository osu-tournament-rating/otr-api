using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class OAuthClientService(IOAuthClientRepository repository, IMapper mapper) : IOAuthClientService
{
    private readonly IOAuthClientRepository _repository = repository;

    public async Task<OAuthClientDTO?> GetAsync(int clientId)
    {
        OAuthClient? client = await _repository.GetAsync(clientId);
        if (client == null)
        {
            return null;
        }

        return new OAuthClientDTO
        {
            ClientId = clientId,
            Scopes = client.Scopes,
            RateLimitOverrides = client.RateLimitOverrides
        };
    }

    public async Task<OAuthClientCreatedDTO> CreateAsync(int userId, string secret, params string[] scopes)
    {
        var client = new OAuthClient { Scopes = scopes, Secret = secret, UserId = userId };

        OAuthClient newClient = await _repository.CreateAsync(client);

        OAuthClientCreatedDTO dto = mapper.Map<OAuthClientCreatedDTO>(newClient);
        dto.ClientSecret = secret;

        return dto;
    }

    public async Task<bool> SecretInUse(string clientSecret) =>
        await _repository.SecretInUseAsync(clientSecret);

    public async Task<OAuthClientDTO?> SetRatelimitOverridesAsync(int clientId, RateLimitOverrides rateLimitOverrides) =>
        mapper.Map<OAuthClientDTO>(await _repository.SetRatelimitOverridesAsync(clientId, rateLimitOverrides));

    public async Task<bool> ExistsAsync(int id, int userId) =>
        await _repository.ExistsAsync(id, userId);

    public async Task<bool> DeleteAsync(int id) =>
        (await _repository.DeleteAsync(id)).HasValue;
}
