using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using Azure.Core;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace API.Repositories.Implementations;

public class PlayerMatchStatsRepository : IPlayerMatchStatsRepository
{
	private readonly OtrContext _context;
	public PlayerMatchStatsRepository(OtrContext context) { _context = context; }

	public async Task<IEnumerable<PlayerMatchStats>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax) => await _context.PlayerMatchStats
	                                                                                                                                                .Where(stats =>
		                                                                                                                                                stats.PlayerId ==
		                                                                                                                                                playerId &&
		                                                                                                                                                stats.Match.Mode == mode &&
		                                                                                                                                                stats.Match.StartTime >=
		                                                                                                                                                dateMin &&
		                                                                                                                                                stats.Match.StartTime <=
		                                                                                                                                                dateMax)
	                                                                                                                                                .OrderBy(x => x.Match.StartTime)
	                                                                                                                                                .ToListAsync();

	public async Task<IEnumerable<PlayerMatchStats>> TeammateStatsAsync(int playerId, int teammateId, int mode, DateTime dateMin,
		DateTime dateMax) => await _context.PlayerMatchStats
		                                   .Where(stats => stats.PlayerId == playerId &&
		                                                   stats.TeammateIds.Contains(teammateId) &&
		                                                   stats.Match.Mode == mode &&
		                                                   stats.Match.StartTime >= dateMin &&
		                                                   stats.Match.StartTime <= dateMax)
		                                   .OrderBy(x => x.Match.StartTime)
		                                   .ToListAsync();

	public async Task<IEnumerable<PlayerMatchStats>> OpponentStatsAsync(int playerId, int opponentId, int mode, DateTime dateMin,
		DateTime dateMax) => await _context.PlayerMatchStats
		                                   .Where(stats => stats.PlayerId == playerId &&
		                                                   stats.OpponentIds.Contains(opponentId) &&
		                                                   stats.Match.Mode == mode &&
		                                                   stats.Match.StartTime >= dateMin &&
		                                                   stats.Match.StartTime <= dateMax)
		                                   .OrderBy(x => x.Match.StartTime)
		                                   .ToListAsync();

	public async Task<PlayerModStatsDTO> GetModStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		using (var command = _context.Database.GetDbConnection().CreateCommand())
		{
			const string sql = """
			                   WITH ModCombinations AS (
			                   			    SELECT
			                   			        CASE
			                   			            WHEN (COALESCE(ms.enabled_mods, 0) & 1024) > 0 OR (g.mods & 1024) > 0 THEN 'FL'
			                   			            WHEN (COALESCE(ms.enabled_mods, 0) & 72) = 72 OR (g.mods & 72) = 72 THEN 'HDDT'
			                   			            WHEN (COALESCE(ms.enabled_mods, 0) & 64) > 0 OR (g.mods & 64) > 0 THEN 'DT'
			                   			            WHEN (COALESCE(ms.enabled_mods, 0) & 24) = 24 OR (g.mods & 24) = 24 THEN 'HDHR'
			                   			            WHEN (COALESCE(ms.enabled_mods, 0) & 16) > 0 OR (g.mods & 16) > 0 THEN 'HR'
			                   			            WHEN (COALESCE(ms.enabled_mods, 0) & 10) = 10 OR (g.mods & 10) = 10 THEN 'HDEZ'
			                   			            WHEN (COALESCE(ms.enabled_mods, 0) & 8) > 0 OR (g.mods & 8) > 0 THEN 'HD'
			                   			            WHEN (COALESCE(ms.enabled_mods, 0) & 2) > 0 OR (g.mods & 2) > 0 THEN 'EZ'
			                   			            WHEN COALESCE(ms.enabled_mods, g.mods) = 1 THEN 'NM'
			                   			            ELSE 'NM'
			                   			            END AS ModType,
			                   			        p.id as pid,
			                   			        p.username as puser,
			                   			        g.id AS gameid,
			                   			        (SELECT ARRAY[p.id] <@ gwr.winners AS player_won)
			                   			    FROM match_scores ms
			                   			             JOIN public.games g ON ms.game_id = g.id
			                   			             JOIN players p ON ms.player_id = p.id
			                   			             JOIN game_win_records gwr ON gwr.game_id = g.id
			                   			             JOIN matches m ON g.match_id = m.id
			                   			    WHERE p.id = @playerId AND g.verification_status = 0 AND m.mode = @mode AND m.start_time >= @dateMin AND m.start_time <= @dateMax
			                   			), ModStats AS (
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
			                   			    pid,
			                   			    puser,
			                   			    ModType,
			                   			    GamesWon,
			                   			    GamesPlayed,
			                   			    CASE WHEN GamesPlayed > 0 THEN ROUND(GamesWon::NUMERIC / GamesPlayed, 2) ELSE 0 END AS WinRate
			                   			FROM ModStats;
			                   """;
		
			command.CommandType = CommandType.Text;
			command.CommandText = sql;

			command.Parameters.Add(new NpgsqlParameter<int>("playerId", playerId));
			command.Parameters.Add(new NpgsqlParameter<int>("mode", mode));
			command.Parameters.Add(new NpgsqlParameter<DateTime>("dateMin", dateMin));
			command.Parameters.Add(new NpgsqlParameter<DateTime>("dateMax", dateMax));

			await _context.Database.OpenConnectionAsync();

			using (var result = await command.ExecuteReaderAsync())
			{
				var pms = new PlayerModStatsDTO();
				while (await result.ReadAsync())
				{
					string modType = await result.GetFieldValueAsync<string>("ModType");
					int gamesPlayed = await result.GetFieldValueAsync<int>("GamesPlayed");
					int gamesWon = await result.GetFieldValueAsync<int>("GamesWon");
					double winrate = await result.GetFieldValueAsync<double>("WinRate");
					
					var dto = new ModStatsDTO
					{
						GamesPlayed = gamesPlayed,
						GamesWon = gamesWon,
						Winrate = winrate
					};
					
					switch (modType)
					{
						case "NM":
							pms.PlayedNM = dto;
							break;
						case "EZ":
							pms.PlayedEZ = dto;
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

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE player_match_stats RESTART IDENTITY;");

	public async Task<int> CountMatchesPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;

		return await _context.PlayerMatchStats.Where(x => x.PlayerId == playerId &&
		                                                  x.Match.Mode == mode &&
		                                                  x.Match.StartTime >= dateMin &&
		                                                  x.Match.StartTime <= dateMax)
		                     .CountAsync();
	}

	public async Task<int> CountMatchesWonAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;

		return await _context.PlayerMatchStats.Where(x => x.PlayerId == playerId &&
		                                                  x.Match.Mode == mode &&
		                                                  x.Won &&
		                                                  x.Match.StartTime >= dateMin &&
		                                                  x.Match.StartTime <= dateMax)
		                     .CountAsync();
	}

	public async Task<double> GlobalWinrateAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
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