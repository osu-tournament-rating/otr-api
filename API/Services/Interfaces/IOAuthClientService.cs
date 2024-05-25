using API.DTOs;
using Database.Entities;

namespace API.Services.Interfaces;

public interface IOAuthClientService
{
    /// <summary>
    /// Denotes a client exists for the given id and is owned by userId
    /// </summary>
    Task<bool> ExistsAsync(int id, int userId);

    /// <summary>
    /// Gets an OAuthClient for the given id
    /// </summary>
    Task<OAuthClientDTO?> GetAsync(int id);

    /// <summary>
    /// Creates an OAuthClient for the given user and scopes
    /// </summary>
    /// <param name="userId">The id of the user that owns this client</param>
    /// <param name="scopes">The scopes to assign to the client</param>
    Task<OAuthClientCreatedDTO> CreateAsync(int userId, params string[] scopes);

    /// <summary>
    /// Deletes a client with the given id
    /// </summary>
    /// <returns>True if successful</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Updates the rate limit overrides for a client with the given id
    /// </summary>
    /// <param name="id">Id of the client</param>
    /// <param name="rateLimitOverrides">The new <see cref="RateLimitOverrides"/></param>
    /// <returns>
    /// The client with updated rate limit overrides,
    /// or null if any errors occur
    /// </returns>
    Task<OAuthClientDTO?> SetRatelimitOverridesAsync(int id, RateLimitOverrides rateLimitOverrides);

    /// <summary>
    /// Resets the secret of a client with the given id
    /// </summary>
    /// <param name="id">Id of the client</param>
    /// <returns>
    /// The client with new secret in plaintext (un-hashed),
    /// or null if a client does not exist
    /// </returns>
    Task<OAuthClientCreatedDTO?> ResetSecretAsync(int id);
}
