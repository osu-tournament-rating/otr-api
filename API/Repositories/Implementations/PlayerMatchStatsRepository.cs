using System.Data;
using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace API.Repositories.Implementations;

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
        using (System.Data.Common.DbCommand command = _context.Database.GetDbConnection().CreateCommand())
        {
            const string sql = """
                -- Potential results:
                -- NM, EZ, HD, HR, HDHR, DT, HDDT, FL, HDEZ
                WITH ModCombinations AS (
                    SELECT
                        p.id as pid,
                        p.username as puser,
                        CASE
                            WHEN (COALESCE(ms.enabled_mods, 0) & 1024) > 0 OR (g.mods & 1024) > 0 THEN 'FL'
                            WHEN (COALESCE(ms.enabled_mods, 0) & 256) > 0 OR (g.mods & 256) > 0 THEN 'HT'
                            WHEN (COALESCE(ms.enabled_mods, 0) & 72) = 72 OR (g.mods & 72) = 72 THEN 'HDDT'
                            WHEN (COALESCE(ms.enabled_mods, 0) & 64) > 0 OR (g.mods & 64) > 0 THEN 'DT'
                            WHEN (COALESCE(ms.enabled_mods, 0) & 24) = 24 OR (g.mods & 24) = 24 THEN 'HDHR'
                            WHEN (COALESCE(ms.enabled_mods, 0) & 16) > 0 OR (g.mods & 16) > 0 THEN 'HR'
                            WHEN (COALESCE(ms.enabled_mods, 0) & 10) = 10 OR (g.mods & 10) = 10 THEN 'EZHD'
                            WHEN (COALESCE(ms.enabled_mods, 0) & 8) > 0 OR (g.mods & 8) > 0 THEN 'HD'
                            WHEN (COALESCE(ms.enabled_mods, 0) & 2) > 0 OR (g.mods & 2) > 0 THEN 'EZ'
                            ELSE 'NM'
                            END AS ModType,
                        ms.score AS Score,
                        (SELECT ARRAY[p.id] <@ gwr.winners AS player_won)
                    FROM match_scores ms
                             JOIN public.games g ON ms.game_id = g.id
                             JOIN players p ON ms.player_id = p.id
                             JOIN game_win_records gwr ON gwr.game_id = g.id
                             JOIN matches m ON g.match_id = m.id
                             JOIN tournaments t ON t.id = m.tournament_id
                    WHERE p.id = @playerId AND g.verification_status = 0 AND m.verification_status = 0 AND t.mode = @mode AND m.start_time >= @dateMin AND m.start_time <= @dateMax
                ),
                     NormalizedScores AS (
                         SELECT
                             pid,
                             puser,
                             ModType,
                             AVG(Score / CASE
                                             WHEN ModType = 'FL' THEN 1.12
                                             WHEN ModType = 'HT' THEN 0.3
                                             WHEN ModType = 'HDDT' THEN 1.1872
                                             WHEN ModType = 'DT' THEN 1.12
                                             WHEN ModType = 'HDHR' THEN 1.166
                                             WHEN ModType = 'HR' THEN 1.1
                                             WHEN ModType = 'EZHD' THEN 0.53
                                             WHEN ModType = 'HD' THEN 1.06
                                             WHEN ModType = 'EZ' THEN 0.5
                                             ELSE 1
                                 END) AS NormalizedAverageScore
                         FROM ModCombinations
                         GROUP BY pid, puser, ModType
                     ),
                     ModStats AS (
                         SELECT
                             pid,
                             puser,
                             ModType,
                             COUNT(*) AS GamesPlayed,
                             COUNT(CASE WHEN player_won THEN 1 END) AS GamesWon
                         FROM ModCombinations
                         GROUP BY pid, puser, ModType
                     )
                SELECT
                    ns.pid,
                    ns.puser,
                    ns.ModType,
                    ns.NormalizedAverageScore,
                    ms.GamesPlayed,
                    ms.GamesWon,
                    CASE WHEN ms.GamesPlayed > 0 THEN ROUND(ms.GamesWon::NUMERIC / ms.GamesPlayed, 5) ELSE 0 END AS WinRate
                FROM NormalizedScores ns
                         JOIN ModStats ms ON ns.pid = ms.pid AND ns.puser = ms.puser AND ns.ModType = ms.ModType;
                """;

            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            command.Parameters.Add(new NpgsqlParameter<int>("playerId", playerId));
            command.Parameters.Add(new NpgsqlParameter<int>("mode", mode));
            command.Parameters.Add(new NpgsqlParameter<DateTime>("dateMin", dateMin));
            command.Parameters.Add(new NpgsqlParameter<DateTime>("dateMax", dateMax));

            await _context.Database.OpenConnectionAsync();

            using (System.Data.Common.DbDataReader result = await command.ExecuteReaderAsync())
            {
                var pms = new PlayerModStatsDTO();
                while (await result.ReadAsync())
                {
                    string modType = await result.GetFieldValueAsync<string>("modtype");
                    int gamesPlayed = await result.GetFieldValueAsync<int>("gamesplayed");
                    int gamesWon = await result.GetFieldValueAsync<int>("gameswon");
                    double winrate = await result.GetFieldValueAsync<double>("winrate");
                    double normalizedAverageScore = await result.GetFieldValueAsync<double>(
                        "normalizedaveragescore"
                    );

                    var dto = new ModStatsDTO
                    {
                        GamesPlayed = gamesPlayed,
                        GamesWon = gamesWon,
                        Winrate = winrate,
                        NormalizedAverageScore = normalizedAverageScore
                    };

                    switch (modType)
                    {
                        case "NM":
                            pms.PlayedNM = dto;
                            break;
                        case "EZ":
                            pms.PlayedEZ = dto;
                            break;
                        case "HT":
                            pms.PlayedHT = dto;
                            break;
                        case "HD":
                            pms.PlayedHD = dto;
                            break;
                        case "HR":
                            pms.PlayedHR = dto;
                            break;
                        case "FL":
                            pms.PlayedFL = dto;
                            break;
                        case "DT":
                            pms.PlayedDT = dto;
                            break;
                        case "HDHR":
                            pms.PlayedHDHR = dto;
                            break;
                        case "HDDT":
                            pms.PlayedHDDT = dto;
                            break;
                        case "HDEZ":
                            pms.PlayedHDEZ = dto;
                            break;
                    }
                }

                return pms;
            }
        }
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

        int matchesPlayed = await CountMatchesPlayedAsync(playerId, mode, dateMin, dateMax);
        int matchesWon = await CountMatchesWonAsync(playerId, mode, dateMin, dateMax);

        if (matchesPlayed == 0)
        {
            return 0;
        }

        return matchesWon / (double)matchesPlayed;
    }
}
