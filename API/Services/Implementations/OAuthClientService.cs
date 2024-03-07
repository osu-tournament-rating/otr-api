using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class OAuthClientService(IOAuthClientRepository repository) : IOAuthClientService
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
            ClientSecret = client.Secret,
            Scopes = client.Scopes
        };
    }

    public async Task<OAuthClientDTO> CreateAsync(int userId, string secret, params string[] scopes)
    {
        var client = new OAuthClient
        {
            Scopes = scopes,
            Secret = secret,
            UserId = userId
        };

        OAuthClient newClient = await _repository.CreateAsync(client);

        return new OAuthClientDTO
        {
            ClientId = newClient.Id,
            ClientSecret = newClient.Secret,
            Scopes = newClient.Scopes
        };
    }

    public async Task<bool> SecretInUse(string clientSecret)
    {
        return await _repository.SecretInUseAsync(clientSecret);
    }

    public async Task<bool> ValidateAsync(int clientId, string clientSecret)
    {
        return await _repository.ValidateAsync(clientId, clientSecret);
    }
}
