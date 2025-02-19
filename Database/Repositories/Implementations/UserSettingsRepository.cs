using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class UserSettingsRepository(OtrContext context, IPlayersRepository playerRepository) : RepositoryBase<UserSettings>(context), IUserSettingsRepository
{
    private readonly OtrContext _context = context;

    public async Task<UserSettings?> GetByUserIdAsync(int userId) =>
        await _context.UserSettings.FirstOrDefaultAsync(us => us.UserId == userId);

    public async Task<UserSettings> GenerateDefaultAsync(int playerId)
    {
        Player? player = await playerRepository.GetAsync(id: playerId);

        return new UserSettings() { DefaultRuleset = player?.DefaultRuleset ?? Ruleset.Osu };
    }
}
