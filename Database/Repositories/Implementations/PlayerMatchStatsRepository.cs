using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

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

    private async Task<int> CountMatchesWonAsync(
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
        int matchesPlayed = await CountMatchesPlayedAsync(playerId, ruleset, dateMin, dateMax);
        int matchesWon = await CountMatchesWonAsync(playerId, ruleset, dateMin, dateMax);

        if (matchesPlayed == 0)
        {
            return 0;
        }

        return matchesWon / (double)matchesPlayed;
    }
}
