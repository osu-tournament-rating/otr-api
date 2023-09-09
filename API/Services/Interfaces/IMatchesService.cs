using API.DTOs;
using API.Enums;
using API.Osu.Multiplayer;

namespace API.Services.Interfaces;

public interface IMatchesService : IService<Models.Match>
{
	Task<IEnumerable<MatchDTO>> GetAllAsync(bool onlyIncludeFiltered);
	Task<MatchDTO?> GetByOsuMatchIdAsync(long osuMatchId);
	Task<Models.Match?> GetFirstPendingOrDefaultAsync();
	Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds);

	/// <summary>
	///  Used to queue up matches for verification.
	/// </summary>
	/// <returns>Number of rows inserted</returns>
	Task<int> InsertFromIdBatchAsync(IEnumerable<Models.Match> matches);

	Task<bool> CreateFromApiMatchAsync(OsuApiMatchData osuMatch);
	Task<int> UpdateVerificationStatusAsync(long matchId, VerificationStatus status, MatchVerificationSource source, string? info = null);
}