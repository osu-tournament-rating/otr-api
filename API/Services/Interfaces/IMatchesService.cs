using API.DTOs;
using API.Enums;

namespace API.Services.Interfaces;

public interface IMatchesService
{
    /// <summary>
    /// Creates matches
    /// </summary>
    /// <param name="tournamentId">Id of the parent tournament</param>
    /// <param name="submitterId">Id of the submitting user</param>
    /// <param name="matchIds">List of match ids</param>
    /// <param name="verify">Submitter is a match verifier</param>
    /// <param name="verificationSource">Source of verification (int representation of <see cref="MatchVerificationSource"/></param>
    /// <returns>Location information for the created matches, or null if parent tournament does not exist</returns>
    Task<IEnumerable<MatchCreatedResultDTO>?> CreateAsync(
        int tournamentId,
        int submitterId,
        IEnumerable<long> matchIds,
        bool verify,
        int? verificationSource
    );

    /// <summary>
    /// Marks matches as needing automated checks
    /// </summary>
    /// <param name="invalidOnly">If true, this method only applies to matches that are not Verified or PreVerified</param>
    /// <returns></returns>
    Task RefreshAutomationChecks(bool invalidOnly = true);

    Task<IEnumerable<int>> GetAllIdsAsync(bool onlyIncludeFiltered);
    Task<MatchDTO?> GetByOsuIdAsync(long osuMatchId);
    Task<MatchDTO?> GetAsync(int id, bool filterInvalid = true);
    Task<MatchDTO?> UpdateVerificationStatusAsync(int id, MatchVerificationStatus verificationStatus,
        MatchVerificationSource verificationSource, string? info = null, int? verifierId = null);
    Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        int mode,
        DateTime start,
        DateTime end
    );

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
