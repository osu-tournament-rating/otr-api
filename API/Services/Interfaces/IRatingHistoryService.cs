using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingHistoryService : IService<RatingHistory>
{
	public Task<IEnumerable<RatingHistory>> GetAllForPlayerAsync(int playerId);
	/// <summary>
	/// THIS IS A DANGEROUS COMMAND!!!
	/// This will delete all of the contents of the table and replace it with the given data.
	/// </summary>
	public Task ReplaceBatchAsync(IEnumerable<RatingHistory> histories);

	public Task TruncateAsync();
	public Task InsertAsync(IEnumerable<RatingHistory> histories);
}