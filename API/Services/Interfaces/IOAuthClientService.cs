using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IOAuthClientService
{
    /// <summary>
    ///     Checks whether the given clientId and clientSecret match a valid client.
    /// </summary>
    /// <param name="clientId">The id of the client</param>
    /// <param name="clientSecret">The client secret</param>
    /// <returns>true if the clientId and clientSecret are in the database, false otherwise.</returns>
    Task<bool> ValidateAsync(int clientId, string clientSecret);

    /// <summary>
    /// Gets an OAuthClient that matches the given client id, if it exists.
    /// </summary>
    Task<OAuthClientDTO?> GetAsync(int clientId);

    /// <summary>
    /// Creates an OAuthClient for the given user and scopes.
    /// </summary>
    /// <param name="userId">The id of the user that owns this client</param>
    /// <param name="secret">The client secret</param>
    /// <param name="scopes">The scopes this client has access to</param>
    Task<OAuthClientDTO> CreateAsync(int userId, string secret, params string[] scopes);

    /// <summary>
    /// Returns true if there already exists a client with this secret.
    /// </summary>
    /// <param name="clientSecret">The new secret to check</param>
    Task<bool> SecretInUse(string clientSecret);

    /// <summary>
    /// Applies the provided <see cref="RateLimitOverrides"/> to the
    /// client which has the id of <see cref="clientId"/>
    /// </summary>
    /// <param name="clientId">The id of the client</param>
    /// <param name="rateLimitOverrides">The new <see cref="RateLimitOverrides"/></param>
    /// <returns>A valid <see cref="OAuthClientDTO"/> with the current state of the client,
    /// null if any errors are encountered.</returns>
    Task<OAuthClientDTO?> SetRatelimitOverridesAsync(int clientId, RateLimitOverrides rateLimitOverrides);
}
