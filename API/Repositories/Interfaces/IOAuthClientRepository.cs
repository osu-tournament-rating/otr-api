using Database.Entities;
using Database.Repositories.Interfaces;

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
    /// <param name="rateLimitOverride">The new rate limit</param>
    /// <returns>The updated client</returns>
    Task<OAuthClient?> SetRateLimitOverrideAsync(int clientId, int rateLimitOverride);

    /// <summary>
    /// Generates a new alpha-numeric client secret (size 50 chars)
    /// </summary>
    string GenerateClientSecret();
}
