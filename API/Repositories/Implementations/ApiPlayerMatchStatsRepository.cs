using API.DTOs;
using API.Repositories.Interfaces;
using Database;
using Database.Enums;
using Database.Enums.Verification;
using Database.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class ApiPlayerMatchStatsRepository(OtrContext context) : PlayerMatchStatsRepository(context), IApiPlayerMatchStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<PlayerModStatsDTO> GetModStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var modStats = await _context.GameScores
            .AsNoTracking()
            .Include(ms => ms.Game)
            .ThenInclude(g => g.Match)
            .ThenInclude(m => m.Tournament)
            .Include(ms => ms.Game)
            .ThenInclude(g => g.WinRecord)
            // Filter for player, verified, ruleset, date range
            .Where(ms =>
                ms.PlayerId == playerId
                && ms.Game.VerificationStatus == VerificationStatus.Verified
                && ms.Game.Match.VerificationStatus == VerificationStatus.Verified
                && ms.Game.WinRecord != null
                && ms.Game.Match.Tournament.Ruleset == (Ruleset)ruleset
                && ms.Game.Match.StartTime >= dateMin
                && ms.Game.Match.EndTime <= dateMax
            )
            // Determine mods, score, and if game was won
            .Select(ms => new
            {
                // Match score mods populated for free mod, else game (lobby) mods
                ModType = (Mods?)ms.Mods ?? ms.Game.Mods,
                ms.Score,
                PlayerWon = ms.Game.WinRecord!.WinnerRoster.Contains(playerId)
            })
            // Group by mods
            .GroupBy(g => g.ModType & ~Mods.NoFail).Select(g => new
            {
                ModType = g.Key,
                Stats = new ModStatsDTO
                {
                    GamesPlayed = g.Count(),
                    GamesWon = g.Count(x => x.PlayerWon),
                    // Avoid div by zero
                    WinRate = g.Any()
                        ? (double)g.Count(x => x.PlayerWon) / g.Count()
                        : 0,
                    NormalizedAverageScore = Math.Round(g.Average(x => x.Score / (
                        g.Key == Mods.Easy ? ModScoreMultipliers.Easy :
                        g.Key == Mods.Hidden ? ModScoreMultipliers.Hidden :
                        g.Key == Mods.HardRock ? ModScoreMultipliers.HardRock :
                        g.Key == Mods.HalfTime ? ModScoreMultipliers.HalfTime :
                        g.Key == Mods.DoubleTime ? ModScoreMultipliers.DoubleTime :
                        g.Key == Mods.Flashlight ? ModScoreMultipliers.Flashlight :
                        g.Key == (Mods.Hidden | Mods.DoubleTime) ? ModScoreMultipliers.HiddenDoubleTime :
                        g.Key == (Mods.Hidden | Mods.HardRock) ? ModScoreMultipliers.HiddenHardRock :
                        g.Key == (Mods.Hidden | Mods.Easy) ? ModScoreMultipliers.HiddenEasy :
                        ModScoreMultipliers.NoMod
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
                case Mods.None:
                    playerModStats.PlayedNM = obj.Stats;
                    break;
                case Mods.DoubleTime:
                    playerModStats.PlayedDT = obj.Stats;
                    break;
                case Mods.HardRock:
                    playerModStats.PlayedHR = obj.Stats;
                    break;
                case Mods.Hidden:
                    playerModStats.PlayedHD = obj.Stats;
                    break;
                case Mods.Easy:
                    playerModStats.PlayedEZ = obj.Stats;
                    break;
                case Mods.Flashlight:
                    playerModStats.PlayedFL = obj.Stats;
                    break;
                case Mods.HalfTime:
                    playerModStats.PlayedHT = obj.Stats;
                    break;
                case Mods.Hidden | Mods.DoubleTime:
                    playerModStats.PlayedHDDT = obj.Stats;
                    break;
                case Mods.Hidden | Mods.HardRock:
                    playerModStats.PlayedHDHR = obj.Stats;
                    break;
                case Mods.Hidden | Mods.Easy:
                    playerModStats.PlayedHDEZ = obj.Stats;
                    break;
            }
        }

        return playerModStats;
    }
}
