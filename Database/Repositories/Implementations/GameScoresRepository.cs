using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

/// <summary>
/// Repository for managing <see cref="GameScore"/> entities
/// </summary>
[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class GameScoresRepository(OtrContext context) : RepositoryBase<GameScore>(context), IGameScoresRepository
{
    private readonly OtrContext _context = context;

    public override async Task<GameScore?> GetAsync(int id)
    {
        IQueryable<GameScore> query = _context.GameScores
            .IncludeAdminNotes<GameScore, GameScoreAdminNote>();

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

    public async Task<int> DeleteByMatchAndPlayerAsync(int matchId, int playerId)
    {
        // Load the entities that will be deleted to ensure auditing is triggered
        List<GameScore> scoresToDelete = await _context.GameScores
            .Where(gs => gs.Game.MatchId == matchId && gs.PlayerId == playerId)
            .ToListAsync();

        if (scoresToDelete.Count == 0)
        {
            return 0;
        }

        // Remove entities through change tracking to trigger auditing
        _context.GameScores.RemoveRange(scoresToDelete);
        await _context.SaveChangesAsync();

        return scoresToDelete.Count;
    }
}
