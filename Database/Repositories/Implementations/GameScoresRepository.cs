using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Enums;
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

    public async Task<int> AverageTeammateScoreAsync(long osuPlayerId, Ruleset ruleset, DateTime fromTime)
    {
        return (int)await _context
            .GameScores.WhereVerified()
            .AfterDate(fromTime)
            .WhereRuleset(ruleset)
            .WhereTeammateOf(osuPlayerId)
            .Select(ms => ms.Score)
            .AverageAsync();
    }

    public Task<int> AverageOpponentScoreAsync(long osuPlayerId, Ruleset ruleset, DateTime fromTime)
    {
        // TODO: rewrite
        // List<long> oppScoresHeadToHead = await _context
        //     .GameScores.WhereVerified()
        //     .After(fromTime)
        //     .WhereRuleset(ruleset)
        //     .WhereHeadToHead()
        //     .WhereOpponent(osuPlayerId)
        //     .Select(ms => ms.Score)
        //     .ToListAsync();
        //
        // List<long> oppScoresTeamVs = await _context
        //     .GameScores.WhereVerified()
        //     .After(fromTime)
        //     .WhereRuleset(ruleset)
        //     .WhereTeamVs()
        //     .WhereOpponent(osuPlayerId)
        //     .Select(ms => ms.Score)
        //     .ToListAsync();
        //
        // IEnumerable<long> oppScores = oppScoresHeadToHead.Concat(oppScoresTeamVs);
        // return (int)oppScores.Average();
        return Task.FromResult(1);
    }

    public async Task<int> AverageModScoreAsync(
        int playerId,
        Ruleset ruleset,
        int mods,
        DateTime dateMin,
        DateTime dateMax)
    {
        return (int)
            await _context
                .GameScores.WhereVerified()
                .WhereRuleset(ruleset)
                .WhereMods((Mods)mods)
                .WherePlayerId(playerId)
                .WhereDateRange(dateMin, dateMax)
                .Select(x => x.Score)
                .DefaultIfEmpty()
                .AverageAsync();
    }

    public async Task<int> CountModScoresAsync(
        int playerId,
        Ruleset ruleset,
        int mods,
        DateTime dateMin,
        DateTime dateMax)
    {
        return await _context
            .GameScores.WhereVerified()
            .WhereRuleset(ruleset)
            .WhereMods((Mods)mods)
            .WherePlayerId(playerId)
            .WhereDateRange(dateMin, dateMax)
            .CountAsync();
    }
}
