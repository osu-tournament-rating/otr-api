using API.Repositories.Interfaces;
using Database;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class UserSettingsRepository(OtrContext context, IPlayerRepository playerRepository) : RepositoryBase<UserSettings>(context), IUserSettingsRepository
{
    private readonly OtrContext _context = context;

    public async Task<UserSettings?> GetByUserIdAsync(int userId) =>
        await _context.UserSettings.FirstOrDefaultAsync(us => us.UserId == userId);

    public async Task<UserSettings> GenerateDefaultAsync(int playerId)
    {
        Player? player = await playerRepository.GetAsync(playerId);

        return new UserSettings() { DefaultRuleset = player?.Ruleset ?? Ruleset.Standard };
    }
}
