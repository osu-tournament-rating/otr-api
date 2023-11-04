using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerMatchStatsRepository
{
	/// <summary>
	/// A list of all matches played by a player in a given mode between two dates. Ordered by match start time.
	/// </summary>
	/// <param name="playerId"></param>
	/// <param name="mode"></param>
	/// <param name="dateMin"></param>
	/// <param name="dateMax"></param>
	/// <returns></returns>
	Task<IEnumerable<PlayerMatchStats>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
	Task<IEnumerable<PlayerMatchStats>> TeammateStatsAsync(int playerId, int teammateId, int mode, DateTime dateMin, DateTime dateMax);
	Task<IEnumerable<PlayerMatchStats>> OpponentStatsAsync(int playerId, int opponentId, int mode, DateTime dateMin, DateTime dateMax);

	/// <summary>
	/// Returns whether the player won the match
	/// </summary>
	Task<bool> WonAsync(int playerId, int matchId);
	Task InsertAsync(PlayerMatchStats item);
	Task InsertAsync(IEnumerable<PlayerMatchStats> items);
	Task TruncateAsync();
	Task<int> CountMatchesPlayedAsync(int playerId, int mode);
	Task<double> WinRateAsync(int playerId, int mode);
}