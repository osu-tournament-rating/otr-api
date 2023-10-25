using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IPlayerStatisticsService
{
	Task<PlayerStatisticsDTO> GetAsync(long osuPlayerId, int mode, DateTime dateMin, DateTime dateMax);
	Task InsertAsync(PlayerMatchStatistics postBody);
	Task InsertAsync(MatchRatingStatistics postBody);
	Task TruncateAsync();
}