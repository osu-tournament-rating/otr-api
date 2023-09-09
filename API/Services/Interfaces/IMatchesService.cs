using API.Enums;
using API.Osu.Multiplayer;

namespace API.Services.Interfaces;

public interface IMatchesService : IService<Models.Match>
{
	Task<IEnumerable<Models.Match>> GetAllAsync(bool onlyIncludeFiltered);
	Task<Models.Match?> GetByOsuGameIdAsync(long osuGameId);
	Task<IEnumerable<Models.Match>?> GetAllPendingVerificationAsync();
	Task<Models.Match?> GetFirstPendingOrDefaultAsync();
	Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds);
	/// <summary>
	/// Used to queue up matches for verification.
	/// </summary>
	/// <returns>Number of rows inserted</returns>
	Task<int> InsertFromIdBatchAsync(IEnumerable<Models.Match> matches);
	/// <summary>
	/// Creates a match if it doesn't already exist.
	/// </summary>
	/// <param name="match"></param>
	/// <returns>Primary key if created, otherwise null</returns>
	Task<int?> CreateIfNotExistsAsync(Models.Match match);
	Task<bool> CreateFromApiMatchAsync(OsuApiMatchData osuMatch);
	Task<int> UpdateVerificationStatusAsync(long matchId, VerificationStatus status, MatchVerificationSource source, string? info = null);
	Task<IEnumerable<Models.Match>> GetForPlayerAsync(int playerId);
}