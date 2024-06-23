using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Enums;
using Database.Extensions;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchScoresRepository(OtrContext context) : RepositoryBase<GameScore>(context), IMatchScoresRepository
{
    private readonly OtrContext _context = context;

    public async Task<int> AverageTeammateScoreAsync(long osuPlayerId, Ruleset ruleset, DateTime fromTime)
    {
        List<long> teammateScores = await _context
            .GameScores.WhereVerified()
            .After(fromTime)
            .WhereRuleset(ruleset)
            .WhereTeammate(osuPlayerId)
            .Select(ms => ms.Score)
            .ToListAsync();
        return (int)teammateScores.Average();
    }

    public async Task<int> AverageOpponentScoreAsync(long osuPlayerId, Ruleset ruleset, DateTime fromTime)
    {
        List<long> oppScoresHeadToHead = await _context
            .GameScores.WhereVerified()
            .After(fromTime)
            .WhereRuleset(ruleset)
            .WhereHeadToHead()
            .WhereOpponent(osuPlayerId)
            .Select(ms => ms.Score)
            .ToListAsync();

        List<long> oppScoresTeamVs = await _context
            .GameScores.WhereVerified()
            .After(fromTime)
            .WhereRuleset(ruleset)
            .WhereTeamVs()
            .WhereOpponent(osuPlayerId)
            .Select(ms => ms.Score)
            .ToListAsync();

        IEnumerable<long> oppScores = oppScoresHeadToHead.Concat(oppScoresTeamVs);
        return (int)oppScores.Average();
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
