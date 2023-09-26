using API.DTOs;

namespace API.Services.Interfaces;

public interface ILeaderboardService
{
	Task<IEnumerable<Unmapped_LeaderboardDTO>> GetLeaderboardAsync(int mode, int page, int pageSize);
}