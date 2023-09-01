using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingsService : IService<Rating>
{
	Task<Rating?> GetForPlayerAsync(int playerId);
	Task UpdateBatchAsync(IEnumerable<Rating> ratings);
	Task<int> InsertOrUpdateForPlayerAsync(int playerId, Rating rating);
	Task<int?> BatchInsertOrUpdateAsync(IEnumerable<Rating> ratings);
}