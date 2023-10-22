using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class PlayerStatisticsService : IPlayerStatisticsService
{
	private readonly IPlayerRepository _playerRepository;
	private readonly IPlayerMatchStatisticsRepository _matchStatsRepository;
	private readonly IPlayerScoreStatsService _playerScoreStatsService;

	public PlayerStatisticsService(IPlayerRepository playerRepository, IPlayerMatchStatisticsRepository matchStatsRepository, IPlayerScoreStatsService playerScoreStatsService)
	{
		_playerRepository = playerRepository;
		_matchStatsRepository = matchStatsRepository;
		_playerScoreStatsService = playerScoreStatsService;
	}
	
	public async Task<PlayerStatisticsDTO> GetAsync(long osuPlayerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		int playerId = await _playerRepository.GetIdByOsuIdAsync(osuPlayerId);
		var matchStats = await GetMatchStatsAsync(playerId, osuPlayerId, mode, dateMin, dateMax);
		var scoreStats = await GetScoreStatsAsync(playerId, mode, dateMin, dateMax);
		
		return new PlayerStatisticsDTO(matchStats, scoreStats);
	}

	public async Task InsertAsync(PlayerMatchStatistics postBody)
	{
		await _matchStatsRepository.InsertAsync(postBody);
	}

	private async Task<PlayerMatchStatisticsDTO?> GetMatchStatsAsync(int id, long osuId, int mode, DateTime dateMin, DateTime dateMax)
	{
		var stats = (await _matchStatsRepository.GetForPlayerAsync(id, mode, dateMin, dateMax)).ToList();

		if (!stats.Any())
		{
			return null;
		}
		
		return new PlayerMatchStatisticsDTO
		{
			HighestRating = (int) stats.Max(x => x.RatingAfter),
			HighestGlobalRank = stats.Min(x => x.GlobalRankAfter),
			HighestCountryRank = stats.Min(x => x.CountryRankAfter),
			HighestPercentile = stats.Max(x => x.PercentileAfter),
			RatingGained = stats.First().RatingAfter - stats.Last().RatingAfter,
			GamesWon = stats.Sum(x => x.GamesWon),
			GamesLost = stats.Sum(x => x.GamesLost),
			MatchesWon = stats.Count(x => x.Won),
			MatchesLost = stats.Count(x => !x.Won),
			AverageTeammateRating = stats.Average(x => x.AverageTeammateRating),
			AverageOpponentRating = stats.Average(x => x.AverageOpponentRating),
			BestWinStreak = GetHighestWinStreak(stats),
			MatchAverageScoreAggregate = stats.Average(x => x.AverageScore),
			MatchAverageAccuracyAggregate = stats.Average(x => x.AverageAccuracy),
			MatchAverageMissesAggregate = stats.Average(x => x.AverageMisses),
			AverageGamesPlayedAggregate = stats.Average(x => x.GamesPlayed),
			AveragePlacingAggregate = stats.Average(x => x.AveragePlacement),
			MostPlayedTeammateName = await _playerRepository.GetUsernameAsync(MostPlayedTeammateId(stats)),
			MostPlayedOpponentName = await _playerRepository.GetUsernameAsync(MostPlayedOpponentId(stats)),
		};
	}
	
	private int GetHighestWinStreak(IEnumerable<PlayerMatchStatistics> stats)
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
	
	private int MostPlayedTeammateId(IEnumerable<PlayerMatchStatistics> stats)
	{
		var teammates = stats.SelectMany(x => x.TeammateIds).GroupBy(x => x).Select(x => new { Id = x.Key, Count = x.Count() }).ToList();
		return teammates.Any() ? teammates.OrderByDescending(x => x.Count).First().Id : 0;
	}
	
	private int MostPlayedOpponentId(IEnumerable<PlayerMatchStatistics> stats)
	{
		var opponents = stats.SelectMany(x => x.OpponentIds).GroupBy(x => x).Select(x => new { Id = x.Key, Count = x.Count() }).ToList();
		return opponents.Any() ? opponents.OrderByDescending(x => x.Count).First().Id : 0;
	}
	
	private async Task<PlayerScoreStatsDTO?> GetScoreStatsAsync(int id, int mode, DateTime dateMin, DateTime dateMax)
	{
		return await _playerScoreStatsService.GetScoreStatsAsync(id, mode, dateMin, dateMax);
	}
}