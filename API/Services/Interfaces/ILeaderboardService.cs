using API.DTOs;
using API.Enums;

namespace API.Services.Interfaces;

public interface ILeaderboardService
{
	public Task<LeaderboardDTO> GetLeaderboardAsync(LeaderboardRequestQueryDTO requestQuery, int? authorizedPlayerId = null);
}