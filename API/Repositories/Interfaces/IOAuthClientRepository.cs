using API.Entities;

namespace API.Repositories.Interfaces;

public interface IOAuthClientRepository : IRepository<OAuthClient>
{
    Task<bool> SecretInUseAsync(string clientSecret);
    Task<bool> ExistsAsync(int clientId, string clientSecret);

    /// <summary>
    /// Denotes a client exists for the given id and userId
    /// </summary>
    Task<bool> ExistsAsync(int id, int userId);
    Task<OAuthClient?> SetRatelimitOverridesAsync(int clientId, RateLimitOverrides rateLimitOverrides);
}
