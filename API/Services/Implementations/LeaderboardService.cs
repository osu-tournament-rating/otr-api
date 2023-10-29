using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;

namespace API.Services.Implementations;

public class LeaderboardService : ILeaderboardService
{
	private readonly IPlayerRepository _playerRepository;
	private readonly IBaseStatsService _baseStatsService;

	public LeaderboardService(IPlayerRepository playerRepository, IBaseStatsService baseStatsService)
	{
		_playerRepository = playerRepository;
		_baseStatsService = baseStatsService;
	}
	
	public async Task<IEnumerable<LeaderboardDTO>> GetLeaderboardAsync(int mode, int page, int pageSize)
	{
		var baseStats = await _baseStatsService.GetLeaderboardAsync(page, pageSize, mode);
		var leaderboard = new List<LeaderboardDTO>();
		foreach (var baseStat in baseStats)
		{
			if (baseStat == null)
			{
				continue;
			}
			
			long osuId = await _playerRepository.GetOsuIdByIdAsync(baseStat.PlayerId);
			string? name = await _playerRepository.GetUsernameAsync(baseStat.PlayerId);
			
			leaderboard.Add(new LeaderboardDTO
			{
				PlayerId = baseStat.PlayerId,
				OsuId = osuId,
				GlobalRank = baseStat.GlobalRank,
				MatchesPlayed = baseStat.MatchesPlayed,
				Name = name ?? "<Unknown>",
				Rating = baseStat.Rating,
				Tier = baseStat.Tier,
				WinRate = baseStat.WinRate
			});
		}

		return leaderboard;
	}
}