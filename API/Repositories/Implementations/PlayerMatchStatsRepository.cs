using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Entities;
using API.Enums;
using API.Osu;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class PlayerMatchStatsRepository(OtrContext context) : IPlayerMatchStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<PlayerMatchStats>> GetForPlayerAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        return await _context
            .PlayerMatchStats.Where(stats =>
                stats.PlayerId == playerId
                && stats.Match.Tournament.Mode == mode
                && stats.Match.StartTime >= dateMin
                && stats.Match.StartTime <= dateMax
            )
            .OrderBy(x => x.Match.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlayerMatchStats>> TeammateStatsAsync(
        int playerId,
        int teammateId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    ) =>
        await _context
            .PlayerMatchStats.Where(stats =>
                stats.PlayerId == playerId
                && stats.TeammateIds.Contains(teammateId)
                && stats.Match.Tournament.Mode == mode
                && stats.Match.StartTime >= dateMin
                && stats.Match.StartTime <= dateMax
            )
            .OrderBy(x => x.Match.StartTime)
            .ToListAsync();

    public async Task<IEnumerable<PlayerMatchStats>> OpponentStatsAsync(
        int playerId,
        int opponentId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    ) =>
        await _context
            .PlayerMatchStats.Where(stats =>
                stats.PlayerId == playerId
                && stats.OpponentIds.Contains(opponentId)
                && stats.Match.Tournament.Mode == mode
                && stats.Match.StartTime >= dateMin
                && stats.Match.StartTime <= dateMax
            )
            .OrderBy(x => x.Match.StartTime)
            .ToListAsync();

    public async Task<PlayerModStatsDTO> GetModStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var modStats = await _context.MatchScores
            // Filter for player, verified, mode, date range
            .Where(ms =>
                ms.PlayerId == playerId
                && ms.Game.Match.VerificationStatus == MatchVerificationStatus.Verified
                && ms.Game.Match.Tournament.Mode == mode
                && ms.Game.Match.StartTime >= dateMin
                && ms.Game.Match.EndTime <= dateMax)
            // Determine mods, score, and if game was won
            .Select(ms => new
            {
                // Match score mods populated for free mod, else game (lobby) mods
                ModType = (OsuEnums.Mods?)ms.EnabledMods ?? ms.Game.Mods,
                ms.Score,
                PlayerWon = ms.Game.WinRecord.Winners.Contains(playerId)
            })
            // Group by mods
            .GroupBy(g => g.ModType & ~OsuEnums.Mods.NoFail)
            // Calculate win rate and average (normalized) score
            .Select(g => new
            {
                ModType = g.Key,
                Stats = new ModStatsDTO
                {
                    GamesPlayed = g.Count(),
                    GamesWon = g.Count(x => x.PlayerWon),
                    // Avoid div by zero
                    Winrate = g.Any()
                        ? (double)g.Count(x => x.PlayerWon) / g.Count()
                        : 0,
                    NormalizedAverageScore = Math.Round(g.Average(x => x.Score / (
                        g.Key == OsuEnums.Mods.Easy ? OsuEnums.ModScoreMultipliers.Easy :
                        g.Key == OsuEnums.Mods.Hidden ? OsuEnums.ModScoreMultipliers.Hidden :
                        g.Key == OsuEnums.Mods.HardRock ? OsuEnums.ModScoreMultipliers.HardRock :
                        g.Key == OsuEnums.Mods.HalfTime ? OsuEnums.ModScoreMultipliers.HalfTime :
                        g.Key == OsuEnums.Mods.DoubleTime ? OsuEnums.ModScoreMultipliers.DoubleTime :
                        g.Key == OsuEnums.Mods.Flashlight ? OsuEnums.ModScoreMultipliers.Flashlight :
                        g.Key == (OsuEnums.Mods.Hidden | OsuEnums.Mods.DoubleTime) ? OsuEnums.ModScoreMultipliers.HiddenDoubleTime :
                        g.Key == (OsuEnums.Mods.Hidden | OsuEnums.Mods.HardRock) ? OsuEnums.ModScoreMultipliers.HiddenHardRock :
                        g.Key == (OsuEnums.Mods.Hidden | OsuEnums.Mods.Easy) ? OsuEnums.ModScoreMultipliers.HiddenEasy :
                        OsuEnums.ModScoreMultipliers.NoMod
                        )))
                }
            })
            .ToListAsync();

        // Combine mod stats into a PlayerModStatsDTO
        PlayerModStatsDTO playerModStats = new();
        foreach (var obj in modStats)
        {
            // Suppression: DTO only stores mods in the switch
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (obj.ModType)
            {
                case OsuEnums.Mods.None:
                    playerModStats.PlayedNM = obj.Stats;
                    break;
                case OsuEnums.Mods.DoubleTime:
                    playerModStats.PlayedDT = obj.Stats;
                    break;
                case OsuEnums.Mods.HardRock:
                    playerModStats.PlayedHR = obj.Stats;
                    break;
                case OsuEnums.Mods.Hidden:
                    playerModStats.PlayedHD = obj.Stats;
                    break;
                case OsuEnums.Mods.Easy:
                    playerModStats.PlayedEZ = obj.Stats;
                    break;
                case OsuEnums.Mods.Flashlight:
                    playerModStats.PlayedFL = obj.Stats;
                    break;
                case OsuEnums.Mods.HalfTime:
                    playerModStats.PlayedHT = obj.Stats;
                    break;
                case OsuEnums.Mods.Hidden | OsuEnums.Mods.DoubleTime:
                    playerModStats.PlayedHDDT = obj.Stats;
                    break;
                case OsuEnums.Mods.Hidden | OsuEnums.Mods.HardRock:
                    playerModStats.PlayedHDHR = obj.Stats;
                    break;
                case OsuEnums.Mods.Hidden | OsuEnums.Mods.Easy:
                    playerModStats.PlayedHDEZ = obj.Stats;
                    break;
            }
        }

        return playerModStats;
    }

    public async Task InsertAsync(IEnumerable<PlayerMatchStats> items)
    {
        await _context.PlayerMatchStats.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }

    public async Task TruncateAsync() =>
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE player_match_stats RESTART IDENTITY;");

    public async Task<int> CountMatchesPlayedAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context
            .PlayerMatchStats.Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Mode == mode
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .CountAsync();
    }

    public async Task<int> CountMatchesWonAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context
            .PlayerMatchStats.Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Mode == mode
                && x.Won
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .CountAsync();
    }

    public async Task<double> GlobalWinrateAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        var matchesPlayed = await CountMatchesPlayedAsync(playerId, mode, dateMin, dateMax);
        var matchesWon = await CountMatchesWonAsync(playerId, mode, dateMin, dateMax);

        if (matchesPlayed == 0)
        {
            return 0;
        }

        return matchesWon / (double)matchesPlayed;
    }
}
