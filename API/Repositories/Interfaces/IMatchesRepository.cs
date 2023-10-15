using API.Entities;
using API.Enums;

namespace API.Repositories.Interfaces;

public interface IMatchesRepository : IRepository<Match>
{
	Task<IEnumerable<Match>> GetAllAsync(bool onlyIncludeFiltered);
	Task<Match?> GetByMatchIdAsync(long matchId);
	Task<IList<Match>> GetMatchesNeedingAutoCheckAsync();
	Task<Match?> GetFirstMatchNeedingApiProcessingAsync();
	Task<Match?> GetFirstMatchNeedingAutoCheckAsync();
	Task<IList<Match>> GetNeedApiProcessingAsync();
	Task<IEnumerable<Match>> GetByMatchIdsAsync(IEnumerable<long> matchIds);

	/// <summary>
	///  Used to queue up matches for verification.
	/// </summary>
	/// <returns>Number of rows inserted</returns>
	Task<int> BatchInsertAsync(IEnumerable<Match> matches);
	Task<int> UpdateVerificationStatusAsync(long matchId, MatchVerificationStatus status, MatchVerificationSource source, string? info = null);
	Task<IEnumerable<Match>> GetPlayerMatchesAsync(long osuId, int mode, DateTime before, DateTime after);
	Task<int> CountMatchWinsAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<int> CountMatchesPlayedAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<double> GetWinRateAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task UpdateAsApiProcessed(Match match);
	Task UpdateAsAutoChecked(Match match);
	Task SetRequireAutoCheckAsync(bool invalidOnly = true);
}