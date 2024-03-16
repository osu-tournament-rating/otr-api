using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class MatchesService(
    ILogger<MatchesService> logger,
    IMatchesRepository matchesRepository,
    ITournamentsRepository tournamentsRepository,
    IMatchDuplicateRepository duplicateRepository,
    IMapper mapper
    ) : IMatchesService
{
    private readonly ILogger<MatchesService> _logger = logger;
    private readonly IMatchesRepository _matchesRepository = matchesRepository;
    private readonly ITournamentsRepository _tournamentsRepository = tournamentsRepository;
    private readonly IMatchDuplicateRepository _duplicateRepository = duplicateRepository;
    private readonly IMapper _mapper = mapper;

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

    public async Task BatchInsertOrUpdateAsync(
        TournamentWebSubmissionDTO tournamentWebSubmissionDto,
        bool verified,
        int? verifier
    )
    {
        var existingMatches = (await _matchesRepository.GetAsync(tournamentWebSubmissionDto.Ids)).ToList();
        Tournament tournament = await _tournamentsRepository.CreateOrUpdateAsync(
            tournamentWebSubmissionDto,
            verified
        );

        // Update the matches that already exist, if we are verified
        if (verified)
        {
            foreach (Match? match in existingMatches)
            {
                match.NeedsAutoCheck = true;
                match.IsApiProcessed = false;
                match.VerificationStatus = (int)MatchVerificationStatus.Verified;
                match.VerificationSource = verifier;
                match.VerifierUserId = tournamentWebSubmissionDto.SubmitterId;
                match.TournamentId = tournament.Id;
                match.SubmitterUserId = tournamentWebSubmissionDto.SubmitterId;

                await _matchesRepository.UpdateAsync(match);
            }
        }

        // Matches that don't exist yet
        var newMatchIds = tournamentWebSubmissionDto
            .Ids.Except(existingMatches.Select(x => x.MatchId))
            .ToList();
        MatchVerificationStatus verificationStatus = verified
            ? MatchVerificationStatus.Verified
            : MatchVerificationStatus.PendingVerification;

        IEnumerable<Match> newMatches = newMatchIds.Select(id => new Match
        {
            MatchId = id,
            VerificationStatus = (int)verificationStatus,
            NeedsAutoCheck = true,
            IsApiProcessed = false,
            VerificationSource = verifier,
            VerifierUserId = verified ? tournamentWebSubmissionDto.SubmitterId : null,
            TournamentId = tournament.Id,
            SubmitterUserId = tournamentWebSubmissionDto.SubmitterId
        });

        int? result = await _matchesRepository.BulkInsertAsync(newMatches);
        if (result > 0)
        {
            _logger.LogInformation(
                "Successfully inserted {Matches} new matches as {Status}",
                result,
                verificationStatus
            );
        }
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
