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
		ValidateRequest(requestQuery);
		
		var leaderboard = new LeaderboardDTO
		{
			Mode = requestQuery.Mode
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
				WinRate = baseStat.Winrate,
				Mode = baseStat.Mode
			});
		}

		leaderboard.PlayerInfo = leaderboardPlayerInfo;
		return leaderboard;
	}

	private void ValidateRequest(LeaderboardRequestQueryDTO query)
	{
		if (query.Filter.MinRank < 1 || query.Filter.MinRank > query.Filter.MaxRank)
		{
			throw new ArgumentException("MinRank must be greater than 0 and less than MaxRank", nameof(query.Filter.MinRank));
		}

		if (query.Filter.MaxRank < 1 || query.Filter.MaxRank < query.Filter.MinRank)
		{
			throw new ArgumentException("MaxRank must be greater than 0 and greater than MinRank", nameof(query.Filter.MaxRank));
		}

		if (query.Filter.MinRating < 0 || query.Filter.MinRating > query.Filter.MaxRating)
		{
			throw new ArgumentException("MinRating must be greater than 0 and less than MaxRating", nameof(query.Filter.MinRating));
		}

		if (query.Filter.MaxRating < 0 || query.Filter.MaxRating < query.Filter.MinRating)
		{
			throw new ArgumentException("MaxRating must be greater than 0 and greater than MinRating", nameof(query.Filter.MaxRating));
		}

		if (query.Filter.MinMatches < 0 || query.Filter.MinMatches > query.Filter.MaxMatches)
		{
			throw new ArgumentException("MinMatches must be greater than 0 and less than MaxMatches", nameof(query.Filter.MinMatches));
		}

		if (query.Filter.MaxMatches < 0 || query.Filter.MaxMatches < query.Filter.MinMatches)
		{
			throw new ArgumentException("MaxMatches must be greater than 0 and greater than MinMatches", nameof(query.Filter.MaxMatches));
		}

		if (query.Filter.MinWinrate < 0 || query.Filter.MinWinrate > query.Filter.MaxWinrate)
		{
			throw new ArgumentException("MinWinrate must be greater than 0 and less than MaxWinrate", nameof(query.Filter.MinWinrate));
		}

		if (query.Filter.MaxWinrate < 0 || query.Filter.MaxWinrate < query.Filter.MinWinrate)
		{
			throw new ArgumentException("MaxWinrate must be greater than 0 and greater than MinWinrate", nameof(query.Filter.MaxWinrate));
		}

		if (query.Filter.MinWinrate > 1 || query.Filter.MaxWinrate > 1)
		{
			throw new ArgumentException("Winrate must be between 0 and 1", nameof(query.Filter.MinWinrate));
		}
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