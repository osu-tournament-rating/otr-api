using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingsService
{
	/// <summary>
	///  Returns a list of all ratings for a player, one for each game mode (if available)
	/// </summary>
	/// <param name="playerId"></param>
	/// <returns></returns>
	Task<IEnumerable<RatingDTO>> GetForPlayerAsync(long osuPlayerId);
	Task<int> BatchInsertAsync(IEnumerable<RatingDTO> ratings);
	Task<IEnumerable<RatingDTO>> GetAllAsync();
	Task TruncateAsync();
	/// <summary>
	/// Returns the creation date of the most recently created rating entry for a player
	/// </summary>
	/// <returns></returns>
	Task<DateTime> GetRecentCreatedDate(long osuPlayerId);
	Task<int?> InsertOrUpdateAsync(int playerId, Rating rating);
}