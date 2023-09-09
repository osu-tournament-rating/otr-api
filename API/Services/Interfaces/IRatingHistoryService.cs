using API.Models;

namespace API.Services.Interfaces;

public interface IRatingHistoryService : IService<RatingHistory>
{
	public Task<IEnumerable<RatingHistory>> GetForPlayerAsync(int playerId);
	public Task ReplaceBatchAsync(IEnumerable<RatingHistory> histories);
	public Task TruncateAsync();
}