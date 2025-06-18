using API.Services.Interfaces;
using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class UserSettingsService(IUserSettingsRepository userSettingsRepository, IUserRepository userRepository) : IUserSettingsService
{
    public async Task<bool> UpdateRulesetAsync(int userId, Ruleset ruleset)
    {
        UserSettings? settings = await userSettingsRepository.GetByUserIdAsync(userId);

        if (settings is null)
        {
            return false;
        }

        settings.DefaultRuleset = ruleset;
        settings.DefaultRulesetIsControlled = true;
        await userSettingsRepository.UpdateAsync(settings);

        return true;
    }

    public async Task<bool> SyncRulesetAsync(int userId)
    {
        User? user = await userRepository.GetAsync(userId);

        if (user?.Settings is null)
        {
            return false;
        }

        user.Settings.DefaultRuleset = user.Player.DefaultRuleset;
        user.Settings.DefaultRulesetIsControlled = false;
        await userRepository.UpdateAsync(user);

        return true;
    }
}
