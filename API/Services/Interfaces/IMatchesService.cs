using API.DTOs;

namespace API.Services.Interfaces;

public interface IMatchesService
{
	/// <summary>
	/// Marks matches as needing automated checks
	/// </summary>
	/// <param name="invalidOnly">If true, this method only applies to matches that are not Verified or PreVerified</param>
	/// <returns></returns>
	Task RefreshAutomationChecks(bool invalidOnly = true);

	Task<IEnumerable<int>> GetAllIdsAsync(bool onlyIncludeFiltered);
	Task<MatchDTO?> GetByOsuIdAsync(long osuMatchId);
	Task<MatchDTO?> GetAsync(int id, bool filterInvalid = true);
	Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(long osuPlayerId, int mode, DateTime start, DateTime end);

	/// <summary>
	/// Inserts or updates based on user input. Only updates if verified is true.
	/// </summary>
	/// <param name="tournamentWebSubmissionDto"></param>
	/// <param name="verified">Whether to mark the matches inserted as verified. Also allows overwriting of existing values.</param>
	/// <param name="verifier">The entity who verified the matches (int representation of <see cref="MatchVerificationSource")/></param>
	/// <returns></returns>
	Task BatchInsertOrUpdateAsync(TournamentWebSubmissionDTO tournamentWebSubmissionDto, bool verified, int? verifier);

	/// <summary>
	/// A unique mapping of osu! match ids to our internal ids.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<MatchIdMappingDTO>> GetIdMappingAsync();

	/// <summary>
	/// Converts a list of match ids to match id objects
	/// </summary>
	/// <param name="ids"></param>
	/// <returns></returns>
	Task<IEnumerable<MatchDTO>> ConvertAsync(IEnumerable<int> ids);

	/// <summary>
	///  Full flow for one-way operation of marking a match as duplicate, reassinging the
	///  appropriate game data, updating the match_duplicate_xref table,
	///  and deleting the duplicate match items.
	///  <param name="confirmedDuplicate">
	///   If true, all <see cref="duplicateIds" /> are confirmed duplicates.
	///   If false, all <see cref="duplicateIds" /> are confirmed to NOT be duplicates.
	///  </param>
	/// </summary>
	Task VerifyDuplicatesAsync(int verifierUserId, int matchRootId, bool confirmedDuplicate);

	Task<IEnumerable<MatchDuplicateCollectionDTO>> GetAllDuplicatesAsync();
}