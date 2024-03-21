using API.Entities;

namespace API.Repositories.Interfaces;

public interface IOAuthClientRepository : IRepository<OAuthClient>
{
    Task<bool> SecretInUseAsync(string clientSecret);
    Task<bool> ExistsAsync(int clientId, string clientSecret);
    Task<OAuthClient?> SetRatelimitOverridesAsync(int clientId, RateLimitOverrides rateLimitOverrides);
}
