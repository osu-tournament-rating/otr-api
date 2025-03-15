using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class GameScoresRepository(OtrContext context) : RepositoryBase<GameScore>(context), IGameScoresRepository
{
    private readonly OtrContext _context = context;

    public async Task<bool> ExistsAsync(long osuId) =>
        await _context.GameScores.AnyAsync(gs => gs.GameId == osuId);

    public override async Task<GameScore?> GetAsync(int id)
    {
        IQueryable<GameScore> query = _context.GameScores
            .Include(gs => gs.AdminNotes);

        return await query.FirstOrDefaultAsync(gs => gs.Id == id);
    }

    public async Task<Dictionary<Mods, int>> GetModFrequenciesAsync(int playerId, Ruleset ruleset, DateTime? dateMin,
        DateTime? dateMax)
    {
        return await
            _context.GameScores
                .ApplyCommonFilters(ruleset, dateMin, dateMax)
                .WherePlayerId(playerId)
                .GroupBy(gs => gs.Mods)
                .ToDictionaryAsync(grouping => grouping.Key, v => v.Count());
    }

    public async Task<Dictionary<Mods, int>> GetAverageModScoresAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax)
    {
        return await
            _context.GameScores
                .ApplyCommonFilters(ruleset, dateMin, dateMax)
                .WherePlayerId(playerId)
                .GroupBy(gs => gs.Mods)
                .ToDictionaryAsync(grouping => grouping.Key, v => (int)v.Average(gs => gs.Score));
    }

    public async Task<int> CountModScoresAsync(
        int playerId,
        Mods mods,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax)
    {
        return await _context
            .GameScores
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .WhereMods(mods)
            .WherePlayerId(playerId)
            .CountAsync();
    }
}
