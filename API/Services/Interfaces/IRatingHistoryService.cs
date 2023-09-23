using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingHistoryService : IService<RatingHistory>
{
	public Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId, int mode, DateTime fromTime);
	public Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId);
	public Task<int> BatchInsertAsync(IEnumerable<RatingHistoryDTO> histories);
	public Task<RatingHistory?> GetOldestForPlayerAsync(long osuId, int mode);
	public Task TruncateAsync();
}