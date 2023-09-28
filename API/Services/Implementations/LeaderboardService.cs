using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;

namespace API.Services.Implementations;

public class LeaderboardService : ILeaderboardService
{
	private readonly IMatchesService _matchesService;
	private readonly IRatingsService _ratingsService;
	private readonly IPlayerService _playerService;

	public LeaderboardService(IMatchesService matchesService, IRatingsService ratingsService, IPlayerService playerService)
	{
		_matchesService = matchesService;
		_ratingsService = ratingsService;
		_playerService = playerService;
	}

	public async Task<IEnumerable<Unmapped_LeaderboardDTO>> GetLeaderboardAsync(int mode, int page, int pageSize)
	{
		var fromTime = DateTime.MinValue; // Beginning of time
		var ratings = await _ratingsService.GetTopRatingsAsync(page, pageSize, mode);
		var leaderboard = new List<Unmapped_LeaderboardDTO>();
		foreach (var rating in ratings)
		{
			var osuId = await _playerService.GetOsuIdByIdAsync(rating.PlayerId);
			var player = await _playerService.GetPlayerByOsuIdAsync(osuId, false, mode);
			var matchesPlayed = await _matchesService.CountMatchesPlayedAsync(osuId, mode, fromTime);
			var winRate = await _matchesService.GetWinRateAsync(osuId, mode, fromTime);
			leaderboard.Add(new Unmapped_LeaderboardDTO
			{
				GlobalRank = await _ratingsService.GetGlobalRankAsync(osuId, mode),
				Name = player.Username,
				Tier = RatingUtils.GetRankingClassName((int)rating.Mu),
				Rating = (int)rating.Mu,
				MatchesPlayed = matchesPlayed,
				WinRate = winRate,
				OsuId = osuId
			});
		}

		return leaderboard;
	}
}