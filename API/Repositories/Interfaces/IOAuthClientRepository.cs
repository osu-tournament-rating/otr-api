using Database.Entities;

namespace API.Repositories.Interfaces;

public interface IOAuthClientRepository : IRepository<OAuthClient>
{
    /// <summary>
    /// Denotes a client exists for the given id and userId
    /// </summary>
    Task<bool> ExistsAsync(int id, int userId);

    /// <summary>
    /// Updates the rate limit overrides of a client for the given id
    /// </summary>
    /// <param name="clientId">Id of the client</param>
    /// <param name="rateLimitOverrides">Updated rate limit overrides</param>
    /// <returns>The updated client</returns>
    Task<OAuthClient?> SetRatelimitOverridesAsync(int clientId, RateLimitOverrides rateLimitOverrides);

    /// <summary>
    /// Generates a new alpha-numeric client secret (size 50 chars)
    /// </summary>
    string GenerateClientSecret();
}
