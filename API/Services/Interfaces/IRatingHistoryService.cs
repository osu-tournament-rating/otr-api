using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingHistoryService : IService<RatingHistory>
{
	public Task<IEnumerable<RatingHistory>> GetAllForPlayerAsync(int playerId);
	public Task AddBatchAsync(IEnumerable<RatingHistory> histories);
}