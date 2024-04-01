using API.DTOs;
using API.Enums;

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
    /// Gets a user's match submissions for the given id
    /// </summary>
    /// <returns>A list of match submissions, or null if not found</returns>
    Task<IEnumerable<MatchSubmissionStatusDTO>?> GetSubmissionsAsync(int id);

    /// <summary>
    /// Rejects all match submissions of a user for the given id
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="verifierId">Id of the user invoking this action</param>
    /// <param name="verificationSource">Int representation of <see cref="MatchVerificationSource"/></param>
    /// <returns>True if successful or the user has no match submissions</returns>
    Task<bool> RejectSubmissionsAsync(int id, int? verifierId, int? verificationSource);

    /// <summary>
    /// Updates a user's scopes
    /// </summary>
    /// <remarks>Replaces existing scopes with provided scopes</remarks>
    /// <returns>A user, or null if not found</returns>
    Task<UserDTO?> UpdateScopesAsync(int id, IEnumerable<string> scopes);
}
