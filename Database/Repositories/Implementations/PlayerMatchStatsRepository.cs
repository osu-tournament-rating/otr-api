using Common.Enums;
using Common.Enums.Verification;
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

        if (playerIdsList.Count == 0)
        {
            return new Dictionary<int, int>();
        }

        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        var matchCounts = await context
            .PlayerMatchStats
            .AsNoTracking()
            .Where(pms =>
                playerIdsList.Contains(pms.PlayerId) &&
                pms.Match.VerificationStatus == VerificationStatus.Verified &&
                pms.Match.Tournament.Ruleset == ruleset &&
                pms.Match.StartTime >= dateMin &&
                pms.Match.StartTime <= dateMax)
            .GroupBy(pms => pms.PlayerId)
            .Select(g => new { PlayerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PlayerId, x => x.Count);

        // Initialize result with all requested player IDs (including those with 0 matches)
        var result = playerIdsList.ToDictionary(id => id, id => matchCounts.GetValueOrDefault(id, 0));

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

        // Calculate winrate directly in the database query
        var winrates = await context
            .PlayerMatchStats
            .AsNoTracking()
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(x => playerIdsList.Contains(x.PlayerId))
            .GroupBy(x => x.PlayerId)
            .Select(g => new
            {
                PlayerId = g.Key,
                Winrate = g.Count() > 0
                    ? g.Count(x => x.Won) / (double)g.Count()
                    : 0.0
            })
            .ToDictionaryAsync(x => x.PlayerId, x => x.Winrate);

        // Ensure all requested players are in the result with a winrate of 0.0 if they haven't played
        return playerIdsList.ToDictionary(id => id, id => winrates.GetValueOrDefault(id, 0.0));
    }
}
