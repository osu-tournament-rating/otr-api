using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MatchRatingStatsRepository : IMatchRatingStatsRepository
{
	private readonly OtrContext _context;
	public MatchRatingStatsRepository(OtrContext context) { _context = context; }

	public async Task<IEnumerable<MatchRatingStats>> GetForPlayerAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;

		return await _context.MatchRatingStats
		                     .Where(x => x.PlayerId == playerId &&
		                                 x.Match.Mode == mode &&
		                                 x.Match.StartTime >= dateMin &&
		                                 x.Match.StartTime <= dateMax)
		                     .Include(x => x.Match)
		                     .ThenInclude(x => x.Tournament)
		                     .ToListAsync();
	}

	public async Task InsertAsync(MatchRatingStats item)
	{
		await _context.MatchRatingStats.AddAsync(item);
		await _context.SaveChangesAsync();
	}

	public async Task InsertAsync(IEnumerable<MatchRatingStats> items)
	{
		await _context.MatchRatingStats.AddRangeAsync(items);
		await _context.SaveChangesAsync();
	}

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE match_rating_stats RESTART IDENTITY");

	public async Task<int> HighestGlobalRankAsync(int playerId, int mode, DateTime? dateMin, DateTime? dateMax)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;
		
		return await _context.MatchRatingStats
		                     .Where(x => x.PlayerId == playerId &&
		                                 x.Match.Mode == mode &&
		                                 x.Match.StartTime != null &&
		                                 x.Match.StartTime >= dateMin &&
		                                 x.Match.StartTime <= dateMax)
		                     .Select(x => x.GlobalRankAfter)
		                     .DefaultIfEmpty()
		                     .MaxAsync();
	}

	public async Task<int> HighestCountryRankAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;
		
		return await _context.MatchRatingStats
		                     .Where(x =>
			                     x.PlayerId == playerId &&
			                     x.Match.Mode == mode &&
			                     x.Match.StartTime != null &&
			                     x.Match.StartTime >= dateMin &&
			                     x.Match.StartTime <= dateMax)
		                     .Select(x => x.CountryRankAfter)
		                     .DefaultIfEmpty()
		                     .MaxAsync();
	}

	public async Task<DateTime?> GetOldestForPlayerAsync(int playerId, int mode) => await _context.MatchRatingStats
	                                                                                              .Where(x => x.PlayerId == playerId &&
	                                                                                                          x.Match.Mode == mode &&
	                                                                                                          x.Match.StartTime != null)
	                                                                                              .Select(x => x.Match.StartTime)
	                                                                                              .MinAsync();

	public async Task<IEnumerable<MatchRatingStats>> TeammateRatingStatsAsync(int playerId, int teammateId, int mode, DateTime dateMin,
		DateTime dateMax) => await _context.MatchRatingStats
		                                   .Where(mrs => mrs.PlayerId == playerId)
		                                   .Where(mrs => _context.PlayerMatchStats
		                                                         .Any(pms => pms.PlayerId == mrs.PlayerId &&
		                                                                     pms.TeammateIds.Contains(teammateId) &&
		                                                                     pms.Match.Mode == mode &&
		                                                                     pms.Match.StartTime >= dateMin &&
		                                                                     pms.Match.StartTime <= dateMax))
		                                   .Distinct()
		                                   .ToListAsync();

	public async Task<IEnumerable<MatchRatingStats>> OpponentRatingStatsAsync(int playerId, int opponentId, int mode, DateTime dateMin,
		DateTime dateMax) => await _context.MatchRatingStats
		                                   .Where(mrs => mrs.PlayerId == playerId)
		                                   .Where(mrs => _context.PlayerMatchStats
		                                                         .Any(pms => pms.PlayerId == mrs.PlayerId &&
		                                                                     pms.OpponentIds.Contains(opponentId) &&
		                                                                     pms.Match.Mode == mode &&
		                                                                     pms.Match.StartTime >= dateMin &&
		                                                                     pms.Match.StartTime <= dateMax))
		                                   .Distinct()
		                                   .ToListAsync();

	public async Task<PlayerRankChartDTO> GetRankChartAsync(int playerId, int mode, LeaderboardChartType chartType, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;
		
		var chartData = await _context.MatchRatingStats
		                              .Include(x => x.Match)
		                              .ThenInclude(x => x.Tournament)
		                              .Where(x => x.PlayerId == playerId &&
		                                          x.Match.Tournament != null &&
		                                          x.Match.Mode == mode &&
		                                          x.Match.StartTime != null &&
		                                          x.Match.StartTime >= dateMin &&
		                                          x.Match.StartTime <= dateMax)
		                              .Select(x => new RankChartDataPointDTO
		                              {
			                              TournamentName = x.Match.Tournament!.Name,
			                              MatchName = x.Match.Name ?? "Undefined",
			                              Rank = chartType == LeaderboardChartType.Global ? x.GlobalRankAfter : x.CountryRankAfter,
			                              RankChange = chartType == LeaderboardChartType.Global ? x.GlobalRankChange : x.CountryRankChange
		                              })
		                              .ToListAsync();

		return new PlayerRankChartDTO
		{
			ChartData = chartData
		};
	}
}