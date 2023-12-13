using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class PlayerStatsService : IPlayerStatsService
{
	private readonly IBaseStatsService _baseStatsService;
	private readonly IGameWinRecordsRepository _gameWinRecordsRepository;
	private readonly IMapper _mapper;
	private readonly IPlayerMatchStatsRepository _matchStatsRepository;
	private readonly IPlayerRepository _playerRepository;
	private readonly IRatingAdjustmentsRepository _ratingAdjustmentsRepository;
	private readonly IMatchRatingStatsRepository _ratingStatsRepository;
	private readonly ITournamentsRepository _tournamentsRepository;

	public PlayerStatsService(IPlayerRepository playerRepository, IPlayerMatchStatsRepository matchStatsRepository,
		IMatchRatingStatsRepository ratingStatsRepository, ITournamentsRepository tournamentsRepository,
		IBaseStatsService baseStatsService, IRatingAdjustmentsRepository ratingAdjustmentsRepository, IGameWinRecordsRepository gameWinRecordsRepository, IMapper mapper)
	{
		_playerRepository = playerRepository;
		_matchStatsRepository = matchStatsRepository;
		_ratingStatsRepository = ratingStatsRepository;
		_tournamentsRepository = tournamentsRepository;
		_baseStatsService = baseStatsService;
		_ratingAdjustmentsRepository = ratingAdjustmentsRepository;
		_gameWinRecordsRepository = gameWinRecordsRepository;
		_mapper = mapper;
	}

	public async Task<PlayerTeammateComparisonDTO> GetTeammateComparisonAsync(int playerId, int teammateId, int mode, DateTime dateMin,
		DateTime dateMax)
	{
		var teammateRatingStats = (await _ratingStatsRepository.TeammateRatingStatsAsync(playerId, teammateId, mode, dateMin, dateMax)).ToList();
		var teammateMatchStats = (await _matchStatsRepository.TeammateStatsAsync(playerId, teammateId, mode, dateMin, dateMax)).ToList();

		int matchesPlayed = teammateMatchStats.Count;
		int matchesWon = teammateMatchStats.Sum(x => x.Won ? 1 : 0);
		int matchesLost = teammateMatchStats.Sum(x => x.Won ? 0 : 1);
		double winRate = matchesWon / (double)matchesPlayed;

		return new PlayerTeammateComparisonDTO
		{
			PlayerId = playerId,
			TeammateId = teammateId,
			GamesPlayed = teammateMatchStats.Sum(x => x.GamesPlayed),
			MatchesPlayed = matchesPlayed,
			MatchesWon = matchesWon,
			MatchesLost = matchesLost,
			RatingDelta = teammateRatingStats.Sum(x => x.RatingChange),
			WinRate = winRate
		};
	}

	public async Task<PlayerOpponentComparisonDTO> GetOpponentComparisonAsync(int playerId, int opponentId, int mode, DateTime dateMin,
		DateTime dateMax)
	{
		var opponentRatingStats = (await _ratingStatsRepository.OpponentRatingStatsAsync(playerId, opponentId, mode, dateMin, dateMax)).ToList();
		var opponentMatchStats = (await _matchStatsRepository.OpponentStatsAsync(playerId, opponentId, mode, dateMin, dateMax)).ToList();

		int matchesWon = opponentMatchStats.Sum(x => x.Won ? 1 : 0);
		int matchesPlayed = opponentMatchStats.Count;
		double winRate = matchesWon / (double)matchesPlayed;

		return new PlayerOpponentComparisonDTO
		{
			PlayerId = playerId,
			OpponentId = opponentId,
			GamesPlayed = opponentMatchStats.Sum(x => x.GamesPlayed),
			MatchesPlayed = matchesPlayed,
			MatchesWon = matchesWon,
			MatchesLost = opponentMatchStats.Sum(x => x.Won ? 0 : 1),
			RatingDelta = opponentRatingStats.Sum(x => x.RatingChange),
			WinRate = winRate
		};
	}

	public async Task<PlayerRankChartDTO> GetRankChartAsync(int playerId, int mode, LeaderboardChartType chartType, DateTime? dateMin = null,
		DateTime? dateMax = null) => await _ratingStatsRepository.GetRankChartAsync(playerId, mode, chartType, dateMin, dateMax);

	public async Task<PlayerStatsDTO> GetAsync(int playerId, int? comparerId, int mode, DateTime? dateMin = null,
		DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;

		var baseStats = await GetBaseStatsAsync(playerId, mode);
		var matchStats = await GetMatchStatsAsync(playerId, mode, dateMin.Value, dateMax.Value);
		var modStats = await GetModStatsAsync(playerId, mode, dateMin.Value, dateMax.Value);
		var tournamentStats = await GetTournamentStatsAsync(playerId, mode, dateMin.Value, dateMax.Value);
		var ratingStats = await GetRatingStatsAsync(playerId, mode, dateMin.Value, dateMax.Value);

		PlayerTeammateComparisonDTO? teammateComparison = null;
		PlayerOpponentComparisonDTO? opponentComparison = null;

		if (comparerId != null)
		{
			teammateComparison = await GetTeammateComparisonAsync(playerId, comparerId.Value, mode, dateMin.Value, dateMax.Value);
			opponentComparison = await GetOpponentComparisonAsync(playerId, comparerId.Value, mode, dateMin.Value, dateMax.Value);
		}

		return new PlayerStatsDTO(baseStats, matchStats, modStats, tournamentStats,
			ratingStats,
			teammateComparison, opponentComparison);
	}

	public async Task BatchInsertAsync(IEnumerable<PlayerMatchStatsDTO> postBody)
	{
		var items = new List<PlayerMatchStats>();

		foreach (var item in postBody)
		{
			var stats = new PlayerMatchStats
			{
				PlayerId = item.PlayerId,
				MatchId = item.MatchId,
				Won = item.Won,
				AverageScore = item.AverageScore,
				AverageMisses = item.AverageMisses,
				AverageAccuracy = item.AverageAccuracy,
				GamesPlayed = item.GamesPlayed,
				AveragePlacement = item.AveragePlacement,
				GamesWon = item.GamesWon,
				GamesLost = item.GamesLost,
				TeammateIds = item.TeammateIds,
				OpponentIds = item.OpponentIds
			};

			items.Add(stats);
		}

		await _matchStatsRepository.InsertAsync(items);
	}

	public async Task BatchInsertAsync(IEnumerable<MatchRatingStatsDTO> postBody)
	{
		var items = new List<MatchRatingStats>();
		foreach (var item in postBody)
		{
			var stats = new MatchRatingStats
			{
				PlayerId = item.PlayerId,
				MatchId = item.MatchId,
				MatchCost = item.MatchCost,
				RatingBefore = item.RatingBefore,
				RatingAfter = item.RatingAfter,
				RatingChange = item.RatingChange,
				VolatilityBefore = item.VolatilityBefore,
				VolatilityAfter = item.VolatilityAfter,
				VolatilityChange = item.VolatilityChange,
				GlobalRankBefore = item.GlobalRankBefore,
				GlobalRankAfter = item.GlobalRankAfter,
				GlobalRankChange = item.GlobalRankChange,
				CountryRankBefore = item.CountryRankBefore,
				CountryRankAfter = item.CountryRankAfter,
				CountryRankChange = item.CountryRankChange,
				PercentileBefore = item.PercentileBefore,
				PercentileAfter = item.PercentileAfter,
				PercentileChange = item.PercentileChange,
				AverageTeammateRating = item.AverageTeammateRating,
				AverageOpponentRating = item.AverageOpponentRating
			};

			items.Add(stats);
		}

		await _ratingStatsRepository.InsertAsync(items);
	}

	public async Task BatchInsertAsync(IEnumerable<BaseStatsPostDTO> postBody) => await _baseStatsService.BatchInsertAsync(postBody);
	public async Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody) => await _ratingAdjustmentsRepository.BatchInsertAsync(postBody);
	public async Task BatchInsertAsync(IEnumerable<GameWinRecordDTO> postBody) => await _gameWinRecordsRepository.BatchInsertAsync(postBody);

	public async Task TruncateAsync()
	{
		await _baseStatsService.TruncateAsync();
		await _gameWinRecordsRepository.TruncateAsync();
		await _matchStatsRepository.TruncateAsync();
		await _ratingStatsRepository.TruncateAsync();
	}

	public async Task TruncateRatingAdjustmentsAsync() => await _ratingAdjustmentsRepository.TruncateAsync();

	// Returns overall stats for the player, no need to filter by date.
	private async Task<BaseStatsDTO?> GetBaseStatsAsync(int playerId, int mode)
	{
		var dto = await _baseStatsService.GetForPlayerAsync(playerId, mode);

		if (dto == null)
		{
			return null;
		}

		int matchesPlayed = await _matchStatsRepository.CountMatchesPlayedAsync(playerId, mode);
		double winRate = await _matchStatsRepository.GlobalWinrateAsync(playerId, mode);
		int highestRank = await _ratingStatsRepository.HighestGlobalRankAsync(playerId, mode);

		dto.MatchesPlayed = matchesPlayed;
		dto.Winrate = winRate;
		dto.HighestGlobalRank = highestRank;

		return dto;
	}

	private async Task<IEnumerable<MatchRatingStatsDTO>> GetRatingStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		var ratingStats = await _ratingStatsRepository.GetForPlayerAsync(playerId, mode, dateMin, dateMax);
		return _mapper.Map<IEnumerable<MatchRatingStatsDTO>>(ratingStats);
	}

	public async Task<PlayerModStatsDTO> GetModStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax) => await _matchStatsRepository.GetModStatsAsync(playerId, mode, dateMin, dateMax);

	private async Task<PlayerTournamentStatsDTO> GetTournamentStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		const int maxTournaments = 5;

		var tournaments = (await _tournamentsRepository.GetForPlayerAsync(playerId, mode, dateMin, dateMax)).ToList();

		if (!tournaments.Any())
		{
			return new PlayerTournamentStatsDTO();
		}

		var topPerformancesList = await _tournamentsRepository.GetTopPerformancesAsync(maxTournaments, playerId, mode, dateMin, dateMax);

		ICollection<TournamentDTO>? performances = null;
		if (topPerformancesList.Any())
		{
			performances = _mapper.Map<ICollection<TournamentDTO>>(topPerformancesList);
		}

		return new PlayerTournamentStatsDTO
		{
			Count1v1 = tournaments.Count(x => x.TeamSize == 1),
			Count2v2 = tournaments.Count(x => x.TeamSize == 2),
			Count3v3 = tournaments.Count(x => x.TeamSize == 3),
			Count4v4 = tournaments.Count(x => x.TeamSize == 4),
			CountOther = tournaments.Count(x => x.TeamSize > 4),
			TopPerformances = performances
		};
	}

	private async Task<AggregatePlayerMatchStatsDTO?> GetMatchStatsAsync(int id, int mode, DateTime dateMin, DateTime dateMax)
	{
		var matchStats = (await _matchStatsRepository.GetForPlayerAsync(id, mode, dateMin, dateMax)).ToList();
		var ratingStats = (await _ratingStatsRepository.GetForPlayerAsync(id, mode, dateMin, dateMax)).ToList();

		if (!matchStats.Any())
		{
			return new AggregatePlayerMatchStatsDTO();
		}

		return new AggregatePlayerMatchStatsDTO
		{
			AverageMatchCostAggregate = ratingStats.Average(x => x.MatchCost),
			HighestRating = ratingStats.Max(x => x.RatingAfter),
			HighestGlobalRank = ratingStats.Min(x => x.GlobalRankAfter),
			HighestCountryRank = ratingStats.Min(x => x.CountryRankAfter),
			HighestPercentile = ratingStats.Max(x => x.PercentileAfter),
			RatingGained = ratingStats.Last().RatingAfter - ratingStats.First().RatingAfter,
			GamesWon = matchStats.Sum(x => x.GamesWon),
			GamesLost = matchStats.Sum(x => x.GamesLost),
			GamesPlayed = matchStats.Sum(x => x.GamesPlayed),
			MatchesWon = matchStats.Count(x => x.Won),
			MatchesLost = matchStats.Count(x => !x.Won),
			AverageTeammateRating = ratingStats.Average(x => x.AverageTeammateRating),
			AverageOpponentRating = ratingStats.Average(x => x.AverageOpponentRating),
			BestWinStreak = GetHighestWinStreak(matchStats),
			MatchAverageScoreAggregate = matchStats.Average(x => x.AverageScore),
			MatchAverageAccuracyAggregate = matchStats.Average(x => x.AverageAccuracy),
			MatchAverageMissesAggregate = matchStats.Average(x => x.AverageMisses),
			AverageGamesPlayedAggregate = matchStats.Average(x => x.GamesPlayed),
			AveragePlacingAggregate = matchStats.Average(x => x.AveragePlacement),
			MostPlayedTeammateName = await _playerRepository.GetUsernameAsync(MostPlayedTeammateId(matchStats)),
			MostPlayedOpponentName = await _playerRepository.GetUsernameAsync(MostPlayedOpponentId(matchStats)),
			BestTeammateName = string.Empty, // TODO: Implement
			PeriodStart = dateMin,
			PeriodEnd = dateMax
		};
	}

	private int GetHighestWinStreak(IEnumerable<PlayerMatchStats> stats)
	{
		int highest = 0;
		int current = 0;

		foreach (var item in stats)
		{
			if (item.Won)
			{
				current++;
			}
			else
			{
				if (current > highest)
				{
					highest = current;
				}

				current = 0;
			}
		}

		return highest;
	}

	private int? MostPlayedTeammateId(IEnumerable<PlayerMatchStats> stats)
	{
		var teammates = stats.SelectMany(x => x.TeammateIds).GroupBy(x => x).Select(x => new { Id = x.Key, Count = x.Count() }).ToList();
		return teammates.Any() ? teammates.OrderByDescending(x => x.Count).First().Id : null;
	}

	private int? MostPlayedOpponentId(IEnumerable<PlayerMatchStats> stats)
	{
		var opponents = stats.SelectMany(x => x.OpponentIds).GroupBy(x => x).Select(x => new { Id = x.Key, Count = x.Count() }).ToList();
		return opponents.Any() ? opponents.OrderByDescending(x => x.Count).First().Id : null;
	}
}