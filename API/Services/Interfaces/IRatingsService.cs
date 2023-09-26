using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingsService : IService<Rating>
{
	/// <summary>
	///  Returns a list of all ratings for a player, one for each game mode (if available)
	/// </summary>
	/// <param name="playerId"></param>
	/// <returns></returns>
	Task<IEnumerable<RatingDTO>> GetForPlayerAsync(long osuPlayerId);

	Task<int> InsertOrUpdateForPlayerAsync(int playerId, Rating rating);
	Task<int> BatchInsertAsync(IEnumerable<RatingDTO> ratings);
	Task<IEnumerable<RatingDTO>> GetAllAsync();
	Task TruncateAsync();
	Task<int> GetGlobalRankAsync(long osuPlayerId, int mode);
	Task<int> AverageTeammateRating(long osuPlayerId, int mode);
	Task<int> AverageOpponentRating(long osuPlayerId, int mode);
	Task<string?> BestPerformingTeammateNameAsync(long osuPlayerId, int mode, DateTime fromDate);
	Task<string?> WorstPerformingTeammateNameAsync(long osuPlayerId, int mode, DateTime fromDate);
	Task<string?> BestPerformingOpponentNameAsync(long osuPlayerId, int mode, DateTime fromDate);
	Task<string?> WorstPerformingOpponentNameAsync(long osuPlayerId, int mode, DateTime fromDate);
	/// <summary>
	/// Returns the creation date of the most recently create rating entry for a player
	/// </summary>
	/// <returns></returns>
	Task<DateTime> GetRecentCreatedDate(long osuPlayerId);
	/// <summary>
	/// Returns whether the user's rating is trending upwards or downwards
	/// </summary>
	/// <param name="osuId"></param>
	/// <param name="modeInt"></param>
	/// <param name="time"></param>
	/// <returns>True for upwards trend, false for downwards</returns>
	Task<bool> IsRatingPositiveTrendAsync(long osuId, int modeInt, DateTime time);
	Task<IEnumerable<Rating>> GetTopRatingsAsync(int page, int pageSize, int mode);
}