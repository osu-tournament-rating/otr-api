using API.DTOs;
using API.Models;

namespace API.Services.Interfaces;

public interface IRatingHistoryService : IService<RatingHistory>
{
	public Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId);
	public Task ReplaceBatchAsync(IEnumerable<RatingHistory> histories);
	public Task TruncateAsync();
}