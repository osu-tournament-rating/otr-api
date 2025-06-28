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

    public async Task<Dictionary<int, int>> CountMatchesPlayedAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        var playerIdsList = playerIds.ToList();

        var matchCounts = await context
            .PlayerMatchStats
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(x => playerIdsList.Contains(x.PlayerId))
            .GroupBy(x => x.PlayerId)
            .Select(g => new { PlayerId = g.Key, Count = g.Count() })
            .ToListAsync();

        // Create dictionary with all player IDs, defaulting to 0 for those with no matches
        var result = playerIdsList.ToDictionary(id => id, _ => 0);
        foreach (var item in matchCounts)
        {
            result[item.PlayerId] = item.Count;
        }

        return result;
    }

    public async Task<Dictionary<int, double>> GlobalWinrateAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        var playerIdsList = playerIds.ToList();

        var matchStats = await context
            .PlayerMatchStats
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(x => playerIdsList.Contains(x.PlayerId))
            .GroupBy(x => x.PlayerId)
            .Select(g => new
            {
                PlayerId = g.Key,
                MatchesPlayed = g.Count(),
                MatchesWon = g.Count(x => x.Won)
            })
            .ToListAsync();

        // Create dictionary with all player IDs, defaulting to 0.0 for those with no matches
        var result = playerIdsList.ToDictionary(id => id, _ => 0.0);
        foreach (var item in matchStats)
        {
            result[item.PlayerId] = item.MatchesPlayed > 0
                ? item.MatchesWon / (double)item.MatchesPlayed
                : 0.0;
        }

        return result;
    }
}
