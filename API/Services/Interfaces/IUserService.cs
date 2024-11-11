using API.DTOs;
using OsuApiClient.Domain.Osu.Users;

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
    /// Gets a user's player id for the given id
    /// </summary>
    /// <returns>A player id, or null if a user was not found or the user has no player entry </returns>
    Task<int?> GetPlayerIdAsync(int id);

    /// <summary>
    /// Gets a user's OAuth clients for the given id
    /// </summary>
    /// <returns>A list of OAuth clients, or null if not found</returns>
    Task<IEnumerable<OAuthClientDTO>?> GetClientsAsync(int id);

    /// <summary>
    /// Gets a user's match submissions for the given id
    /// </summary>
    /// <returns>A list of match submissions, or null if not found</returns>
    Task<IEnumerable<MatchSubmissionStatusDTO>?> GetSubmissionsAsync(int id);

    /// <summary>
    /// Rejects all match submissions of a user for the given id
    /// </summary>
    /// <param name="id">Id of the target user</param>
    /// <param name="rejecterUserId">Id of the user invoking this action</param>
    /// <returns>True if successful or the user has no match submissions</returns>
    Task<bool> RejectSubmissionsAsync(int id, int? rejecterUserId);

    /// <summary>
    /// Updates a user's scopes
    /// </summary>
    /// <remarks>Replaces existing scopes with provided scopes</remarks>
    /// <returns>A user, or null if not found</returns>
    Task<UserDTO?> UpdateScopesAsync(int id, IEnumerable<string> scopes);
}
