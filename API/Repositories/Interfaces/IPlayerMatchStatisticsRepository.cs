using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerMatchStatisticsRepository
{
	/// <summary>
	/// A list of all matches played by a player in a given mode between two dates. Ordered by match start time.
	/// </summary>
	/// <param name="playerId"></param>
	/// <param name="mode"></param>
	/// <param name="dateMin"></param>
	/// <param name="dateMax"></param>
	/// <returns></returns>
	Task<IEnumerable<PlayerMatchStatistics>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
	Task InsertAsync(PlayerMatchStatistics item);
	Task InsertAsync(IEnumerable<PlayerMatchStatistics> items);
	Task TruncateAsync();
}