using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Services.Implementations;

public class BaseStatsService : IBaseStatsService
{
	private readonly IBaseStatsRepository _baseStatsRepository;
	private readonly IPlayerMatchStatsRepository _matchStatsRepository;
	private readonly IPlayerRepository _playerRepository;
	private readonly IMatchRatingStatsRepository _ratingStatsRepository;

	public BaseStatsService(IBaseStatsRepository baseStatsRepository, IPlayerMatchStatsRepository matchStatsRepository, IMatchRatingStatsRepository ratingStatsRepository,
		IPlayerRepository playerRepository)
	{
		_baseStatsRepository = baseStatsRepository;
		_matchStatsRepository = matchStatsRepository;
		_ratingStatsRepository = ratingStatsRepository;
		_playerRepository = playerRepository;
	}

	public async Task<IEnumerable<BaseStatsDTO?>> GetForPlayerAsync(long osuPlayerId)
	{
		int id = await _playerRepository.GetIdByOsuIdAsync(osuPlayerId);
		var baseStats = await _baseStatsRepository.GetForPlayerAsync(osuPlayerId);
		var ret = new List<BaseStatsDTO?>();

		foreach (var stat in baseStats)
		{
			// One per mode
			ret.Add(await GetForPlayerAsync(stat, id, stat.Mode));
		}

		return ret;
	}

	public async Task<BaseStatsDTO?> GetForPlayerAsync(BaseStats? currentStats, int id, int mode)
	{
		currentStats ??= await _baseStatsRepository.GetForPlayerAsync(id, mode);

		if (currentStats == null)
		{
			return null;
		}

		int matchesPlayed = await _matchStatsRepository.CountMatchesPlayedAsync(id, mode);
		double winRate = await _matchStatsRepository.GlobalWinrateAsync(id, mode);
		int highestGlobalRank = await _ratingStatsRepository.HighestGlobalRankAsync(id, mode);

		return new BaseStatsDTO(id, currentStats.Rating, currentStats.Volatility, currentStats.Mode,
			currentStats.Percentile, matchesPlayed, winRate, highestGlobalRank, currentStats.GlobalRank,
			currentStats.CountryRank, currentStats.MatchCostAverage);
	}

	public async Task<int> BatchInsertAsync(IEnumerable<BaseStatsPostDTO> stats)
	{
		var toInsert = new List<BaseStats>();
		foreach (var item in stats)
		{
			toInsert.Add(new BaseStats
			{
				PlayerId = item.PlayerId,
				MatchCostAverage = item.MatchCostAverage,
				Rating = item.Rating,
				Volatility = item.Volatility,
				Mode = item.Mode,
				Percentile = item.Percentile,
				GlobalRank = item.GlobalRank,
				CountryRank = item.CountryRank
			});
		}

		return await _baseStatsRepository.BatchInsertAsync(toInsert);
	}

	public async Task<IEnumerable<BaseStatsDTO?>> GetLeaderboardAsync(int mode, int page, int pageSize, LeaderboardChartType chartType,
		LeaderboardFilterDTO filter, int? playerId)
	{
		var baseStats = await _baseStatsRepository.GetLeaderboardAsync(page, pageSize, mode, chartType, filter,
			playerId);

		var leaderboard = new List<BaseStatsDTO?>();

		foreach (var baseStat in baseStats)
		{
			leaderboard.Add(await GetForPlayerAsync(baseStat, baseStat.PlayerId, mode));
		}

		return leaderboard;
	}

	public async Task TruncateAsync() => await _baseStatsRepository.TruncateAsync();

	public async Task<int> LeaderboardCountAsync(int requestQueryMode, LeaderboardChartType requestQueryChartType, LeaderboardFilterDTO requestQueryFilter, int? playerId) =>
		await _baseStatsRepository.LeaderboardCountAsync(requestQueryMode, requestQueryChartType, requestQueryFilter, playerId);

	public async Task<LeaderboardFilterDefaultsDTO> LeaderboardFilterDefaultsAsync(int requestQueryMode, LeaderboardChartType requestQueryChartType) => new()
	{
		MaxRating = await _baseStatsRepository.HighestRatingAsync(requestQueryMode),
		MaxMatches = await _baseStatsRepository.HighestMatchesAsync(requestQueryMode),
		MaxRank = await _baseStatsRepository.HighestRankAsync(requestQueryMode)
	};

	public async Task<ActionResult<IEnumerable<double>>> GetHistogramAsync(int mode) => await _baseStatsRepository.GetHistogramAsync(mode);
}