using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class LeaderboardService : ILeaderboardService
{
	private readonly IPlayerRepository _playerRepository;
	private readonly IBaseStatsService _baseStatsService;
	private readonly IMatchRatingStatsRepository _ratingStatsRepository;
	private readonly IPlayerService _playerService;
	private readonly IPlayerStatsService _playerStatsService;

	public LeaderboardService(IPlayerRepository playerRepository, IBaseStatsService baseStatsService, 
		IMatchRatingStatsRepository ratingStatsRepository, IPlayerService playerService, IPlayerStatsService playerStatsService)
	{
		_playerRepository = playerRepository;
		_baseStatsService = baseStatsService;
		_ratingStatsRepository = ratingStatsRepository;
		_playerService = playerService;
		_playerStatsService = playerStatsService;
	}
	
	public async Task<LeaderboardDTO> GetLeaderboardAsync(LeaderboardRequestQueryDTO requestQuery)
	{
		var leaderboard = new LeaderboardDTO
		{
			Mode = requestQuery.Mode,
		};
		
		if (requestQuery.UserId.HasValue)
		{
			int? playerId = await _playerService.GetIdAsync(requestQuery.UserId.Value);

			if (playerId.HasValue)
			{
				leaderboard.PlayerChart = await GetPlayerChartAsync(requestQuery.UserId.Value, requestQuery.Mode, requestQuery.ChartType);
			}
		}
		
		var baseStats = await _baseStatsService.GetLeaderboardAsync(requestQuery.Mode, requestQuery.Page, requestQuery.PageSize, requestQuery.ChartType, requestQuery.Filter);
		var leaderboardPlayerInfo = new List<LeaderboardPlayerInfoDTO>();
		
		foreach (var baseStat in baseStats)
		{
			if (baseStat == null)
			{
				continue;
			}
			
			long osuId = await _playerRepository.GetOsuIdByIdAsync(baseStat.PlayerId);
			string? name = await _playerRepository.GetUsernameAsync(baseStat.PlayerId);
			
			leaderboardPlayerInfo.Add(new LeaderboardPlayerInfoDTO
			{
				PlayerId = baseStat.PlayerId,
				OsuId = osuId,
				GlobalRank = baseStat.GlobalRank,
				MatchesPlayed = baseStat.MatchesPlayed,
				Name = name ?? "<Unknown>",
				Rating = baseStat.Rating,
				Tier = baseStat.Tier,
				WinRate = baseStat.Winrate
			});
		}

		leaderboard.PlayerInfo = leaderboardPlayerInfo;
		return leaderboard;
	}
	
	private async Task<LeaderboardPlayerChartDTO?> GetPlayerChartAsync(int playerId, int mode, LeaderboardChartType chartType)
	{
		var baseStats = await _baseStatsService.GetForPlayerAsync(playerId, mode);

		if (baseStats == null)
		{
			return null;
		}
		
		int rank = chartType switch 
		{
			LeaderboardChartType.Global => baseStats.GlobalRank,
			LeaderboardChartType.Country => baseStats.CountryRank,
			_ => throw new ArgumentOutOfRangeException(nameof(chartType), chartType, null)
		};
		
		int highestRank = chartType switch 
		{
			LeaderboardChartType.Global => await _ratingStatsRepository.HighestGlobalRankAsync(playerId, mode),
			LeaderboardChartType.Country => await _ratingStatsRepository.HighestCountryRankAsync(playerId, mode),
			_ => throw new ArgumentOutOfRangeException(nameof(chartType), chartType, null)
		};
		
		var rankChart = await _playerStatsService.GetRankChartAsync(playerId, mode, chartType);

		return new LeaderboardPlayerChartDTO
		{
			Rank = rank,
			HighestRank = highestRank,
			Percentile = baseStats.Percentile,
			Rating = baseStats.Rating,
			Matches = baseStats.MatchesPlayed,
			Winrate = baseStats.Winrate,
			RankChart = rankChart
		};
	}
}