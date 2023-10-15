using API.DTOs;

namespace API.Services.Interfaces;

public interface IRatingHistoryService
{
	public Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId, int mode, DateTime fromTime);
	public Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId);
	public Task<int> BatchInsertAsync(IEnumerable<RatingHistoryDTO> histories);
	public Task TruncateAsync();
}