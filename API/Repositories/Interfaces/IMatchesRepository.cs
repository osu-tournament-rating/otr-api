using API.DTOs;
using API.Entities;
using API.Enums;

namespace API.Repositories.Interfaces;

public interface IMatchesRepository : IRepository<Match>
{
	Task<Match> GetAsync(int id, bool filterInvalidMatches = true);
	Task<IEnumerable<Match>> GetAsync(IEnumerable<int> ids, bool onlyIncludeFiltered);
	Task<IEnumerable<int>> GetAllAsync(bool filterInvalidMatches);
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
	Task<IEnumerable<MatchIdMappingDTO>> GetIdMappingAsync();

	/// <summary>
	///  Marks all duplicate matches of the <see cref="matchRootId" /> as duplicates. All game and score data from all of the
	///  matches
	///  will be moved to reference the <see cref="root" /> match. All duplicate osu match ids will then
	///  be stored as a <see cref="MatchDuplicate" /> and deleted.
	///  The root and the duplicates both must all have either a matching title, matching osu! id, or both.
	/// </summary>
	/// <param name="matchRootId">The id of the match that all duplicate data will be moved to</param>
	/// <exception cref="Exception">
	///  Thrown if the matches are not from the same tournament or if any of the
	///  matches have not been fully processed.
	///  Thrown if any of the matches in <see cref="duplicates" /> are missing game or score data, or if
	///  the start time of the root is not the earliest start time of both the root and all <see cref="duplicates" />, or if
	///  the
	///  duplicates fail to match the root's title or osu id.
	/// </exception>
	/// <returns></returns>
	Task MergeDuplicatesAsync(int matchRootId);

	/// <summary>
	///  Marks all of the matches in the duplicates list as duplicates of the root.
	///  This does NOT merge the duplicates, it simply marks them for manual review.
	/// </summary>
	/// <param name="root"></param>
	/// <param name="duplicates"></param>
	/// <returns></returns>
	Task MarkSuspectedDuplicatesAsync(Match root, IEnumerable<Match> duplicates);

	Task VerifyDuplicatesAsync(int matchRoot, int userId, bool confirmed);

	/// <summary>
	///  Returns all collections of duplicate matches present in the table.
	///  Each collection represents a group of matches that are duplicates.
	///  Use the <see cref="MergeDuplicatesAsync" /> method after identifying the
	///  root match from the collection to merge the data.
	///  Duplicates are any matches that have identical tournament ids AND either or both
	///  of the following:
	///  1. The MatchId properties are identical
	///  2. The Name properties are identical
	/// </summary>
	/// <returns>A list of duplicate collections</returns>
	Task<IEnumerable<IList<Match>>> GetDuplicateGroupsAsync();
}