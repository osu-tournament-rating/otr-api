using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingHistoryService : IService<RatingHistory>
{
	public Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId, DateTime fromTime);
	public Task<int> BatchInsertAsync(IEnumerable<RatingHistoryDTO> histories);
	public Task TruncateAsync();
}