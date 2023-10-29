using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class PlayerStatsService : IPlayerStatsService
{
	private readonly IMapper _mapper;
	private readonly IPlayerMatchStatsRepository _matchStatsRepository;
	private readonly IPlayerRepository _playerRepository;
	private readonly IPlayerScoreStatsService _playerScoreStatsService;
	private readonly IMatchRatingStatsRepository _ratingStatsRepository;
	private readonly ITournamentsRepository _tournamentsRepository;
	private readonly IBaseStatsService _baseStatsService;

	public PlayerStatsService(IPlayerRepository playerRepository, IPlayerMatchStatsRepository matchStatsRepository,
		IMatchRatingStatsRepository ratingStatsRepository, IPlayerScoreStatsService playerScoreStatsService, ITournamentsRepository tournamentsRepository,
		IBaseStatsService baseStatsService, IMapper mapper)
	{
		_playerRepository = playerRepository;
		_matchStatsRepository = matchStatsRepository;
		_ratingStatsRepository = ratingStatsRepository;
		_playerScoreStatsService = playerScoreStatsService;
		_tournamentsRepository = tournamentsRepository;
		_baseStatsService = baseStatsService;
		_mapper = mapper;
	}

	public async Task<PlayerStatsDTO> GetAsync(long osuPlayerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		int playerId = await _playerRepository.GetIdByOsuIdAsync(osuPlayerId);
		
		var baseStats = await GetBaseStatsAsync(playerId, mode);
		var matchStats = await GetMatchStatsAsync(playerId, mode, dateMin, dateMax);
		var scoreStats = await GetScoreStatsAsync(playerId, mode, dateMin, dateMax);
		var tournamentStats = await GetTournamentStatsAsync(playerId, mode, dateMin, dateMax);
		var ratingStats = await GetRatingStatsAsync(playerId, mode, dateMin, dateMax);

		return new PlayerStatsDTO(baseStats, matchStats, scoreStats, tournamentStats, ratingStats);
	}

	// Returns overall stats for the player, no need to filter by date.
	private async Task<BaseStatsDTO?> GetBaseStatsAsync(int playerId, int mode)
	{
		var dto = await _baseStatsService.GetForPlayerAsync(playerId, mode);

		if (dto == null)
		{
			return null;
		}
		
		int matchesPlayed = await _matchStatsRepository.CountMatchesPlayedAsync(playerId, mode);
		double winRate = await _matchStatsRepository.WinRateAsync(playerId, mode);
		int highestRank = await _ratingStatsRepository.HighestGlobalRankAsync(playerId, mode);
		
		dto.MatchesPlayed = matchesPlayed;
		dto.WinRate = winRate;
		dto.HighestGlobalRank = highestRank;

		return dto;
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

	public async Task TruncateAsync()
	{
		await _baseStatsService.TruncateAsync();
		await _matchStatsRepository.TruncateAsync();
		await _ratingStatsRepository.TruncateAsync();
	}
	
	private async Task<IEnumerable<MatchRatingStatsDTO>> GetRatingStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		var ratingStats = await _ratingStatsRepository.GetForPlayerAsync(playerId, mode, dateMin, dateMax);
		return _mapper.Map<IEnumerable<MatchRatingStatsDTO>>(ratingStats);
	}

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
			return null;
		}
		
		return new AggregatePlayerMatchStatsDTO
		{
			HighestRating = (int)ratingStats.Max(x => x.RatingAfter),
			HighestGlobalRank = ratingStats.Min(x => x.GlobalRankAfter),
			HighestCountryRank = ratingStats.Min(x => x.CountryRankAfter),
			HighestPercentile = ratingStats.Max(x => x.PercentileAfter),
			RatingGained = ratingStats.First().RatingAfter - ratingStats.Last().RatingAfter,
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

	private async Task<PlayerScoreStatsDTO?> GetScoreStatsAsync(int id, int mode, DateTime dateMin, DateTime dateMax) =>
		await _playerScoreStatsService.GetScoreStatsAsync(id, mode, dateMin, dateMax);
}