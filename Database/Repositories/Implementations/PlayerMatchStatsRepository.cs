using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class PlayerMatchStatsRepository(OtrContext context) : IPlayerMatchStatsRepository
{
    public async Task<IEnumerable<PlayerMatchStats>> GetForPlayerAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax
    )
    {
        return await context
            .PlayerMatchStats
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(stats => stats.PlayerId == playerId)
            .OrderBy(x => x.Match.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetTeammateIdsAsync(int playerId, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax)
    {
        return await context
            .PlayerMatchStats
            .Where(pms => pms.PlayerId == playerId)
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .SelectMany(pms => pms.TeammateIds)
            .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetOpponentIdsAsync(int playerId, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax)
    {
        return await context
            .PlayerMatchStats
            .Where(pms => pms.PlayerId == playerId)
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .SelectMany(pms => pms.OpponentIds)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlayerMatchStats>> TeammateStatsAsync(
        int playerId,
        int teammateId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax
    ) =>
        await context
            .PlayerMatchStats
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(stats =>
                stats.PlayerId == playerId
                && stats.TeammateIds.Contains(teammateId)
            )
            .OrderBy(x => x.Match.StartTime)
            .ToListAsync();

    public async Task<IEnumerable<PlayerMatchStats>> OpponentStatsAsync(
        int playerId,
        int opponentId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax
    ) =>
        await context
            .PlayerMatchStats
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(stats =>
                stats.PlayerId == playerId
                && stats.OpponentIds.Contains(opponentId)
            )
            .OrderBy(x => x.Match.StartTime)
            .ToListAsync();

    public async Task<int> CountMatchesPlayedAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        return await context
            .PlayerMatchStats
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(x =>
                x.PlayerId == playerId
            )
            .CountAsync();
    }

    public async Task<int> CountMatchesWonAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        return await context
            .PlayerMatchStats
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(x =>
                x.PlayerId == playerId && x.Won
            )
            .CountAsync();
    }

    public async Task<double> GlobalWinrateAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        var matchesPlayed = await CountMatchesPlayedAsync(playerId, ruleset, dateMin, dateMax);
        var matchesWon = await CountMatchesWonAsync(playerId, ruleset, dateMin, dateMax);

        if (matchesPlayed == 0)
        {
            return 0;
        }

        return matchesWon / (double)matchesPlayed;
    }

    public async Task<Dictionary<int, double>> GetMatchCostsAsync(int playerId, Ruleset ruleset,
        DateTime? dateMin = null, DateTime? dateMax = null) =>
        await context.PlayerMatchStats
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(pms => pms.PlayerId == playerId)
            .ToDictionaryAsync(k => k.MatchId, v => v.MatchCost);
}
