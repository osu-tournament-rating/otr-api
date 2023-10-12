using API.DTOs;
using API.Entities;
using API.Enums;

namespace API.Services.Interfaces;

public interface IMatchesService : IService<Match>
{
	Task RefreshAutomationChecks(bool invalidOnly = true);
	Task<IEnumerable<MatchDTO>> GetAllAsync(bool onlyIncludeFiltered);
	Task<MatchDTO?> GetDTOByOsuMatchIdAsync(long osuMatchId);
	Task<Match?> GetByMatchIdAsync(long matchId);
	Task<IList<Match>> GetMatchesNeedingAutoCheckAsync();
	Task<Match?> GetFirstMatchNeedingApiProcessingAsync();
	Task<Match?> GetFirstMatchNeedingAutoCheckAsync();
	Task<IList<Match>> GetNeedApiProcessingAsync();
	Task<IEnumerable<Match>> CheckExistingAsync(IEnumerable<long> matchIds);

	/// <summary>
	///  Used to queue up matches for verification.
	/// </summary>
	/// <returns>Number of rows inserted</returns>
	Task<int> BatchInsertAsync(IEnumerable<Match> matches);
	Task<int> UpdateVerificationStatusAsync(long matchId, MatchVerificationStatus status, MatchVerificationSource source, string? info = null);
	Task<Unmapped_PlayerMatchesDTO> GetPlayerMatchesAsync(long osuId, DateTime fromTime);
	Task<int> CountMatchWinsAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<IEnumerable<Unmapped_VerifiedTournamentDTO>> GetAllVerifiedTournamentsAsync();
	Task<int> CountMatchesPlayedAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<double> GetWinRateAsync(long osuPlayerId, int mode, DateTime fromTime);
	Task<string?> GetMatchAbbreviationAsync(long osuMatchId);
	Task UpdateAsApiProcessed(Match match);
	Task UpdateAsAutoChecked(Match match);
}