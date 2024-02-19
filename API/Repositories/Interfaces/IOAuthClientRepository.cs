using API.Entities;

namespace API.Repositories.Interfaces;

public interface IOAuthClientRepository : IRepository<OAuthClient>
{
    Task<bool> SecretInUseAsync(string clientSecret);
    Task<bool> ValidateAsync(int clientId, string clientSecret);
}