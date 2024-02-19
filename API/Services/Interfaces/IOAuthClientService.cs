using API.DTOs;

namespace API.Services.Interfaces;

public interface IOAuthClientService
{
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
    Task<OAuthClientDTO?> CreateAsync(int userId, string secret, params string[] scopes);
    /// <summary>
    /// Returns true if there already exists a client with this secret.
    /// </summary>
    /// <param name="clientSecret">The new secret to check</param>
    Task<bool> SecretInUse(string clientSecret);
    /// <summary>
    /// Checks the provided client id & secret are valid.
    /// </summary>
    /// <param name="clientId">The id of the client</param>
    /// <param name="clientSecret">The client secret</param>
    /// <returns>True if the client id & secret provided belong to this user, false otherwise</returns>
    Task<bool> ValidateAsync(int clientId, string clientSecret);
}