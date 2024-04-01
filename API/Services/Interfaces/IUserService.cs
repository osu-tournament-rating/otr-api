using API.DTOs;

namespace API.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Denotes whether a user for the given id exists
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Gets a user for the given id
    /// </summary>
    /// <returns>A user, or null if not found</returns>
    Task<UserDTO?> GetAsync(int id);

    /// <summary>
    /// Gets a user's OAuth clients for the given id
    /// </summary>
    /// <returns>A list of OAuth clients, or null if not found</returns>
    Task<IEnumerable<OAuthClientDTO>?> GetClientsAsync(int id);

    /// <summary>
    /// Updates a user's scopes
    /// </summary>
    /// <remarks>Replaces existing scopes with provided scopes</remarks>
    /// <returns>A user, or null if not found</returns>
    Task<UserDTO?> UpdateScopesAsync(int id, IEnumerable<string> scopes);
}
