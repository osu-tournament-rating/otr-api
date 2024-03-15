using System.Diagnostics.CodeAnalysis;
using API.Entities;
using API.Osu;
using API.Repositories.Interfaces;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchScoresRepository(OtrContext context) : RepositoryBase<MatchScore>(context), IMatchScoresRepository
{
    private readonly OtrContext _context = context;

    public async Task<int> AverageTeammateScoreAsync(long osuPlayerId, int mode, DateTime fromTime)
    {
        List<long> teammateScores = await _context
            .MatchScores.WhereVerified()
            .After(fromTime)
            .WhereMode(mode)
            .WhereTeammate(osuPlayerId)
            .Select(ms => ms.Score)
            .ToListAsync();
        return (int)teammateScores.Average();
    }

    public async Task<int> AverageOpponentScoreAsync(long osuPlayerId, int mode, DateTime fromTime)
    {
        List<long> oppScoresHeadToHead = await _context
            .MatchScores.WhereVerified()
            .After(fromTime)
            .WhereMode(mode)
            .WhereHeadToHead()
            .WhereOpponent(osuPlayerId)
            .Select(ms => ms.Score)
            .ToListAsync();

        List<long> oppScoresTeamVs = await _context
            .MatchScores.WhereVerified()
            .After(fromTime)
            .WhereMode(mode)
            .WhereTeamVs()
            .WhereOpponent(osuPlayerId)
            .Select(ms => ms.Score)
            .ToListAsync();

        IEnumerable<long> oppScores = oppScoresHeadToHead.Concat(oppScoresTeamVs);
        return (int)oppScores.Average();
    }

    public async Task<int> AverageModScoreAsync(
        int playerId,
        int mode,
        int mods,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        return (int)
            await _context
                .MatchScores.WhereVerified()
                .WhereMode(mode)
                .WhereMods((OsuEnums.Mods)mods)
                .WherePlayerId(playerId)
                .WhereDateRange(dateMin, dateMax)
                .Select(x => x.Score)
                .DefaultIfEmpty()
                .AverageAsync();
    }

    public async Task<int> CountModScoresAsync(
        int playerId,
        int mode,
        int mods,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        return await _context
            .MatchScores.WhereVerified()
            .WhereMode(mode)
            .WhereMods((OsuEnums.Mods)mods)
            .WherePlayerId(playerId)
            .WhereDateRange(dateMin, dateMax)
            .CountAsync();
    }
}
