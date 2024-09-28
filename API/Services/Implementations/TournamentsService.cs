using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class TournamentsService(
    ITournamentsRepository tournamentsRepository,
    IMatchesRepository matchesRepository,
    IMapper mapper) : ITournamentsService
{
    public async Task<TournamentCreatedResultDTO> CreateAsync(
        TournamentSubmissionDTO submission,
        int submitterUserId,
        bool preApprove
    )
    {
        // Only create matches that dont already exist
        IEnumerable<long> enumerableMatchIds = submission.Ids.ToList();
        IEnumerable<long> existingMatchIds = (await matchesRepository.GetAsync(enumerableMatchIds))
            .Select(m => m.OsuId)
            .ToList();

        Tournament tournament = await tournamentsRepository.CreateAsync(new Tournament
        {
            Name = submission.Name,
            Abbreviation = submission.Abbreviation,
            ForumUrl = submission.ForumUrl,
            RankRangeLowerBound = submission.RankRangeLowerBound,
            Ruleset = submission.Ruleset,
            LobbySize = submission.LobbySize,
            ProcessingStatus =
                preApprove ? TournamentProcessingStatus.NeedsMatchData : TournamentProcessingStatus.NeedsApproval,
            SubmittedByUserId = submitterUserId,
            Matches = enumerableMatchIds
                .Except(existingMatchIds)
                .Select(matchId => new Match { OsuId = matchId, SubmittedByUserId = submitterUserId }).ToList()
        });
        return mapper.Map<TournamentCreatedResultDTO>(tournament);
    }

    public async Task<bool> ExistsAsync(string name, Ruleset ruleset)
        => await tournamentsRepository.ExistsAsync(name, ruleset);

    public async Task<IEnumerable<TournamentDTO>> ListAsync()
    {
        IEnumerable<Tournament> items = await tournamentsRepository.GetAllAsync();
        items = items.OrderBy(x => x.Name);

        return mapper.Map<IEnumerable<TournamentDTO>>(items);
    }

    public async Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true) =>
        mapper.Map<TournamentDTO?>(await tournamentsRepository.GetAsync(id, eagerLoad));

    public async Task<ICollection<TournamentDTO>> GetAsync(TournamentRequestQueryDTO requestQuery)
    {
        ValidateRequest(requestQuery);

        return mapper.Map<ICollection<TournamentDTO>>(await tournamentsRepository.GetAsync(requestQuery.Page, requestQuery.PageSize,
            requestQuery.Verified));
    }

    public async Task<TournamentDTO?> GetVerifiedAsync(int id) =>
        mapper.Map<TournamentDTO?>(await tournamentsRepository.GetVerifiedAsync(id));

    public async Task<int> CountPlayedAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await tournamentsRepository.CountPlayedAsync(playerId, ruleset, dateMin ?? DateTime.MinValue,
        dateMax ?? DateTime.MaxValue);

    public async Task<TournamentDTO?> UpdateAsync(int id, TournamentDTO wrapper)
    {
        Tournament? existing = await tournamentsRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.Name = wrapper.Name;
        existing.Abbreviation = wrapper.Abbreviation;
        existing.ForumUrl = wrapper.ForumUrl;
        existing.Ruleset = wrapper.Ruleset;
        existing.RankRangeLowerBound = wrapper.RankRangeLowerBound;
        existing.LobbySize = wrapper.LobbySize;

        await tournamentsRepository.UpdateAsync(existing);
        return mapper.Map<TournamentDTO>(existing);
    }

    private static void ValidateRequest(TournamentRequestQueryDTO requestQuery)
    {
        if (requestQuery.Page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(requestQuery.Page), "Page must be greater than 0");
        }

        if (requestQuery.PageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(requestQuery.PageSize), "PageSize must be greater than 0");
        }
    }
}
