using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class MatchesService(
    IMatchesRepository matchesRepository,
    IMatchDuplicateRepository duplicateRepository,
    ITournamentsRepository tournamentsRepository,
    IMapper mapper
) : IMatchesService
{
    private readonly IMatchesRepository _matchesRepository = matchesRepository;
    private readonly IMatchDuplicateRepository _duplicateRepository = duplicateRepository;
    private readonly ITournamentsRepository _tournamentsRepository = tournamentsRepository;
    private readonly IMapper _mapper = mapper;

    // TODO: Refactor to use enums for param "verificationSource"
    public async Task<IEnumerable<MatchCreatedResultDTO>?> CreateAsync(
        int tournamentId,
        int submitterId,
        IEnumerable<long> matchIds,
        bool verify,
        int? verificationSource
    )
    {
        Tournament? tournament = await _tournamentsRepository.GetAsync(tournamentId);
        if (tournament is null)
        {
            return null;
        }

        MatchVerificationStatus verificationStatus = verify
            ? MatchVerificationStatus.Verified
            : MatchVerificationStatus.PendingVerification;

        // Only create matches that dont already exist
        IEnumerable<long> enumerableMatchIds = matchIds.ToList();
        IEnumerable<long> existingMatchIds = (await _matchesRepository.GetAsync(enumerableMatchIds))
            .Select(m => m.MatchId)
            .ToList();

        // Create matches directly on the tournament because we can't access their ids until after the entity is updated
        IEnumerable<long> createdMatchIds = enumerableMatchIds.Except(existingMatchIds).ToList();
        foreach (var matchId in createdMatchIds)
        {
            tournament.Matches.Add(new Match
            {
                MatchId = matchId,
                VerificationStatus = verificationStatus,
                NeedsAutoCheck = true,
                IsApiProcessed = false,
                VerificationSource = (MatchVerificationSource?)verificationSource,
                VerifierUserId = verify ? submitterId : null,
                SubmitterUserId = submitterId
            });
        }

        await _tournamentsRepository.UpdateAsync(tournament);
        IEnumerable<Match> createdMatches = tournament.Matches.Where(m => createdMatchIds.Contains(m.MatchId));

        return _mapper.Map<IEnumerable<MatchCreatedResultDTO>>(createdMatches);
    }

    public async Task<MatchDTO?> GetAsync(int id, bool filterInvalid = true) =>
            _mapper.Map<MatchDTO?>(await _matchesRepository.GetAsync(id, filterInvalid));

    public async Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        int mode,
        DateTime start,
        DateTime end
    )
    {
        IEnumerable<Match> matches = await _matchesRepository.GetPlayerMatchesAsync(osuPlayerId, mode, start, end);
        return _mapper.Map<IEnumerable<MatchDTO>>(matches);
    }

    public async Task<IEnumerable<MatchIdMappingDTO>> GetIdMappingAsync() =>
        await _matchesRepository.GetIdMappingAsync();

    public async Task<IEnumerable<MatchDTO>> ConvertAsync(IEnumerable<int> ids) =>
        _mapper.Map<IEnumerable<MatchDTO>>(await _matchesRepository.GetAsync(ids, true));

    public async Task VerifyDuplicatesAsync(int verifierUserId, int matchRootId, bool confirmedDuplicate)
    {
        // Mark the items as confirmed / denied duplicates
        await _matchesRepository.VerifyDuplicatesAsync(matchRootId, verifierUserId, confirmedDuplicate);

        // If confirmedDuplicate, trigger the update process.
        if (confirmedDuplicate)
        {
            await _matchesRepository.MergeDuplicatesAsync(matchRootId);
        }
    }

    public async Task<IEnumerable<MatchDuplicateCollectionDTO>> GetAllDuplicatesAsync()
    {
        var collections = new List<MatchDuplicateCollectionDTO>();
        IEnumerable<IGrouping<int, MatchDuplicate>> duplicateGroups = (await _duplicateRepository.GetAllUnknownStatusAsync()).GroupBy(x =>
            x.SuspectedDuplicateOf
        );
        foreach (IGrouping<int, MatchDuplicate> dupeGroup in duplicateGroups)
        {
            MatchDTO? root = await GetAsync(dupeGroup.First().SuspectedDuplicateOf, false) ?? throw new Exception("Failed to find root from lookup.");
            var collection = new MatchDuplicateCollectionDTO
            {
                Id = root.Id,
                Name = root.Name ?? string.Empty,
                OsuMatchId = root.MatchId,
                SuspectedDuplicates = new List<MatchDuplicateDTO>()
            };

            foreach (MatchDuplicate? item in dupeGroup)
            {
                if (root == null || item.MatchId == root.Id)
                {
                    continue;
                }

                MatchDTO? duplicateMatchData = await GetByOsuIdAsync(item.OsuMatchId);
                if (duplicateMatchData == null)
                {
                    continue;
                }

                collection.SuspectedDuplicates.Add(
                    new MatchDuplicateDTO
                    {
                        Name = duplicateMatchData.Name ?? string.Empty,
                        OsuMatchId = duplicateMatchData.MatchId,
                        VerifiedByUsername = item.Verifier?.Player.Username,
                        VerifiedAsDuplicate = item.VerifiedAsDuplicate,
                        VerifiedByUserId = item.VerifiedBy
                    }
                );
            }

            collections.Add(collection);
        }

        return collections;
    }

    public async Task RefreshAutomationChecks(bool invalidOnly = true) =>
        await _matchesRepository.SetRequireAutoCheckAsync(invalidOnly);

    public async Task<IEnumerable<int>> GetAllIdsAsync(bool onlyIncludeFiltered)
    {
        return await _matchesRepository.GetAllAsync(onlyIncludeFiltered);
    }

    public async Task<MatchDTO?> GetByOsuIdAsync(long osuMatchId)
    {
        Match? match = await _matchesRepository.GetByMatchIdAsync(osuMatchId);
        return _mapper.Map<MatchDTO?>(match);
    }

    public async Task<MatchDTO> UpdateVerificationStatus(int id, int? verificationStatus)
    {
        Match match = await _matchesRepository.UpdateVerificationStatus(id, verificationStatus);
        return _mapper.Map<MatchDTO>(match);
    }
}
