using Common.Enums.Enums;

namespace API.Services.Interfaces;

public interface IUserSettingsService
{
    /// <summary>
    /// Updates the ruleset of a user
    /// </summary>
    /// <param name="userId">Id of the target user</param>
    /// <param name="ruleset">The new ruleset to assign</param>
    /// <returns>True if successful</returns>
    Task<bool> UpdateRulesetAsync(int userId, Ruleset ruleset);

    /// <summary>
    /// Sync the ruleset of a user with their osu! ruleset
    /// </summary>
    /// <param name="userId">Id of the target user</param>
    /// <returns>True if successful</returns>
    Task<bool> SyncRulesetAsync(int userId);
}
