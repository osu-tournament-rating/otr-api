using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace API.Services.Implementations;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class LeaderboardService : ILeaderboardService
{
	private readonly IBaseStatsService _baseStatsService;
	private readonly IPlayerRepository _playerRepository;
	private readonly IPlayerService _playerService;
	private readonly IPlayerStatsService _playerStatsService;
	private readonly IMatchRatingStatsRepository _ratingStatsRepository;

	public LeaderboardService(IPlayerRepository playerRepository, IBaseStatsService baseStatsService,
		IMatchRatingStatsRepository ratingStatsRepository, IPlayerService playerService, IPlayerStatsService playerStatsService)
	{
		_playerRepository = playerRepository;
		_baseStatsService = baseStatsService;
		_ratingStatsRepository = ratingStatsRepository;
		_playerService = playerService;
		_playerStatsService = playerStatsService;
	}

	public async Task<LeaderboardDTO> GetLeaderboardAsync(LeaderboardRequestQueryDTO requestQuery, int? authorizedPlayerId = null)
	{
		ValidateRequest(requestQuery);

		var leaderboard = new LeaderboardDTO
		{
			Mode = requestQuery.Mode,
			TotalPlayerCount = await _baseStatsService.LeaderboardCountAsync(requestQuery.Mode, requestQuery.ChartType, requestQuery.Filter, authorizedPlayerId),
			FilterDefaults = await _baseStatsService.LeaderboardFilterDefaultsAsync(requestQuery.Mode, requestQuery.ChartType)
		};

		if (authorizedPlayerId.HasValue)
		{
			int? playerId = await _playerService.GetIdAsync(authorizedPlayerId.Value);

			if (playerId.HasValue)
			{
				leaderboard.PlayerChart = await GetPlayerChartAsync(authorizedPlayerId.Value, requestQuery.Mode, requestQuery.ChartType);
			}
		}

		var baseStats = await _baseStatsService.GetLeaderboardAsync(requestQuery.Mode, requestQuery.Page, requestQuery.PageSize, requestQuery.ChartType, requestQuery.Filter,
			authorizedPlayerId);

		var leaderboardPlayerInfo = new List<LeaderboardPlayerInfoDTO>();

		foreach (var baseStat in baseStats)
		{
			if (baseStat == null)
			{
				continue;
			}

			long osuId = await _playerRepository.GetOsuIdAsync(baseStat.PlayerId);
			string? name = await _playerRepository.GetUsernameAsync(baseStat.PlayerId);
			string? country = await _playerRepository.GetCountryAsync(baseStat.PlayerId);

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
				Mode = baseStat.Mode,
				Country = country
			});
		}

		leaderboard.Leaderboard = leaderboardPlayerInfo;
		return leaderboard;
	}

	private void ValidateRequest(LeaderboardRequestQueryDTO query)
	{
		if (query.Filter.MinRank < 1 || query.Filter.MinRank > query.Filter.MaxRank)
		{
			throw new ArgumentException("MinRank must be greater than 0 and less than or equal to MaxRank", nameof(query.Filter.MinRank));
		}

		if (query.Filter.MaxRank < 1 || query.Filter.MaxRank < query.Filter.MinRank)
		{
			throw new ArgumentException("MaxRank must be greater than 0 and greater than or equal to MinRank", nameof(query.Filter.MaxRank));
		}

		if (query.Filter.MinRating < 100 || query.Filter.MinRating > query.Filter.MaxRating)
		{
			throw new ArgumentException("MinRating must be at least 100 and be less than or equal to MaxRating", nameof(query.Filter.MinRating));
		}

		if (query.Filter.MaxRating < 100 || query.Filter.MaxRating < query.Filter.MinRating)
		{
			throw new ArgumentException("MaxRating must be at least 100 and greater than or equal to MinRating", nameof(query.Filter.MaxRating));
		}

		if (query.Filter.MinMatches < 0 || query.Filter.MinMatches > query.Filter.MaxMatches)
		{
			throw new ArgumentException("MinMatches must be at least 0 and less than or equal to MaxMatches", nameof(query.Filter.MinMatches));
		}

		if (query.Filter.MaxMatches < 0 || query.Filter.MaxMatches < query.Filter.MinMatches)
		{
			throw new ArgumentException("MaxMatches must be at least 1 and greater than or equal to MinMatches", nameof(query.Filter.MaxMatches));
		}

		if (query.Filter.MinWinrate < 0 || query.Filter.MinWinrate > query.Filter.MaxWinrate)
		{
			throw new ArgumentException("MinWinrate must be greater than 0 and less than or equal to MaxWinrate", nameof(query.Filter.MinWinrate));
		}

		if (query.Filter.MaxWinrate < 0 || query.Filter.MaxWinrate < query.Filter.MinWinrate)
		{
			throw new ArgumentException("MaxWinrate must be greater than 0 and greater than or equal to MinWinrate", nameof(query.Filter.MaxWinrate));
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