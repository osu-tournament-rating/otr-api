using API.DTOs;
using API.Enums;
using API.Osu.Multiplayer;

namespace API.Services.Interfaces;

public interface IMatchesService : IService<Entities.Match>
{
	Task RefreshAutomationChecks(bool invalidOnly = true);
	Task<IEnumerable<MatchDTO>> GetAllAsync(bool onlyIncludeFiltered);
	Task<MatchDTO?> GetDTOByOsuMatchIdAsync(long osuMatchId);
	Task<Entities.Match?> GetByMatchIdAsync(long matchId);
	Task<IList<Entities.Match>> GetMatchesNeedingAutoCheckAsync();
	Task<Entities.Match?> GetFirstMatchNeedingApiProcessingAsync();
	Task<Entities.Match?> GetFirstMatchNeedingAutoCheckAsync();
	Task<IList<Entities.Match>> GetNeedApiProcessingAsync();
	Task<IEnumerable<Entities.Match>> CheckExistingAsync(IEnumerable<long> matchIds);

	/// <summary>
	///  Used to queue up matches for verification.
	/// </summary>
	/// <returns>Number of rows inserted</returns>
	Task<int> BatchInsertAsync(IEnumerable<Entities.Match> matches);
	Task<int> UpdateVerificationStatusAsync(long matchId, MatchVerificationStatus status, MatchVerificationSource source, string? info = null);
	Task<Unmapped_PlayerMatchesDTO> GetPlayerMatchesAsync(long osuId, DateTime fromTime);
	Task<int> CountMatchWinsAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<IEnumerable<Unmapped_VerifiedTournamentDTO>> GetAllVerifiedTournamentsAsync();
	Task<int> CountMatchesPlayedAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<double> GetWinRateAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<string?> GetMatchAbbreviationAsync(long osuMatchId);
	Task UpdateAsApiProcessed(Entities.Match match);
	Task UpdateAsAutoChecked(Entities.Match match);
}