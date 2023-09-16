using API.DTOs;
using API.Enums;
using API.Osu.Multiplayer;

namespace API.Services.Interfaces;

public interface IMatchesService : IService<Entities.Match>
{
	Task<IEnumerable<MatchDTO>> GetAllAsync(bool onlyIncludeFiltered);
	Task<MatchDTO?> GetByOsuMatchIdAsync(long osuMatchId);
	Task<Entities.Match?> GetFirstPendingUnpopulatedVerifiedOrDefaultAsync();
	Task<IEnumerable<Entities.Match>> CheckExistingAsync(IEnumerable<long> matchIds);

	/// <summary>
	///  Used to queue up matches for verification.
	/// </summary>
	/// <returns>Number of rows inserted</returns>
	Task<int> InsertFromIdBatchAsync(IEnumerable<Entities.Match> matches);

	Task<bool> CreateFromApiMatchAsync(OsuApiMatchData osuMatch);
	Task<int> UpdateVerificationStatusAsync(long matchId, MatchVerificationStatus status, MatchVerificationSource source, string? info = null);
	Task<Unmapped_PlayerMatchesDTO> GetPlayerMatchesAsync(long osuId);
	Task<int> CountMatchWinsAsync(long osuPlayerId, int mode);
}