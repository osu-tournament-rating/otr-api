using API.DTOs;

namespace API.Services.Interfaces;

public interface ILeaderboardService
{
	public Task<IEnumerable<LeaderboardDTO>> GetLeaderboardAsync(int mode, int page, int pageSize);
}