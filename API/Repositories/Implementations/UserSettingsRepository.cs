using API.Entities;
using API.Osu.Enums;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations;

public class UserSettingsRepository(OtrContext context, IPlayerRepository playerRepository) : RepositoryBase<UserSettings>(context), IUserSettingsRepository
{
    public async Task<UserSettings> GenerateDefaultAsync(int playerId)
    {
        Player? player = await playerRepository.GetAsync(playerId);

        return new UserSettings() { DefaultRuleset = player?.Ruleset ?? Ruleset.Standard };
    }
}
