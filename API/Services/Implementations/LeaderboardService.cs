using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;

namespace API.Services.Implementations;

public class LeaderboardService : ILeaderboardService
{
	private readonly IPlayerRepository _playerRepository;
	private readonly IMatchesRepository _matchesRepository;
	private readonly IBaseStatsRepository _baseStatsRepository;

	public LeaderboardService(IPlayerRepository playerRepository, IMatchesRepository matchesRepository, IBaseStatsRepository baseStatsRepository)
	{
		_playerRepository = playerRepository;
		_matchesRepository = matchesRepository;
		_baseStatsRepository = baseStatsRepository;
	}
	
	public async Task<IEnumerable<LeaderboardDTO>> GetLeaderboardAsync(int mode, int page, int pageSize)
	{
		var fromTime = DateTime.MinValue; // Beginning of time
		var ratings = await _baseStatsRepository.GetTopRatingsAsync(page, pageSize, mode);
		var leaderboard = new List<LeaderboardDTO>();
		foreach (var rating in ratings)
		{
			var osuId = await _playerRepository.GetOsuIdByIdAsync(rating.PlayerId);
			var player = await _playerRepository.GetPlayerByOsuIdAsync(osuId, false, mode);
			var matchesPlayed = await _matchesRepository.CountMatchesPlayedAsync(osuId, mode, fromTime);
			var winRate = await _matchesRepository.GetWinRateAsync(osuId, mode, fromTime);
			leaderboard.Add(new LeaderboardDTO
			{
				GlobalRank = await _baseStatsRepository.GetGlobalRankAsync(osuId, mode),
				Name = player.Username,
				Tier = RatingUtils.GetTier((int)rating.Rating),
				Rating = (int)rating.Rating,
				MatchesPlayed = matchesPlayed,
				WinRate = winRate,
				OsuId = osuId
			});
		}

		return leaderboard;
	}
}