using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingsService : IService<Rating>
{
	Task<IEnumerable<Rating>> GetAllForPlayerAsync(int id);
	Task UpdateBatchAsync(IEnumerable<Rating> ratings);
	Task<int> InsertOrUpdateForPlayerAsync(int playerId, Rating rating);
}