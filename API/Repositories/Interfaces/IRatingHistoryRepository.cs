using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface IRatingHistoryRepository : IRepository<RatingHistory>
{
	public Task<IEnumerable<RatingHistory>> GetForPlayerAsync(long osuPlayerId, int mode, DateTime fromTime);
	public Task<IEnumerable<RatingHistory>> GetForPlayerAsync(long osuPlayerId);
	public Task<int> BatchInsertAsync(IEnumerable<RatingHistoryDTO> histories);
	public Task<RatingHistory?> GetOldestForPlayerAsync(long osuId, int mode);
	public Task TruncateAsync();
}