using API.Controllers;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Queries.Filters;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class TournamentsService(
    ITournamentsRepository tournamentsRepository,
    IMatchesRepository matchesRepository,
    IUrlHelperService urlHelperService,
    IMapper mapper
) : ITournamentsService
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
            ProcessingStatus = preApprove ? TournamentProcessingStatus.NeedsMatchData : TournamentProcessingStatus.NeedsApproval,
            SubmittedByUserId = submitterUserId,
            Matches = enumerableMatchIds
                .Except(existingMatchIds)
                .Select(matchId => new Match
                {
                    OsuId = matchId,
                    SubmittedByUserId = submitterUserId
                }).ToList()
        });
        return mapper.Map<TournamentCreatedResultDTO>(tournament);
    }

    public async Task<bool> ExistsAsync(string name, Ruleset ruleset)
        => await tournamentsRepository.ExistsAsync(name, ruleset);

    public async Task<PagedResultDTO<TournamentDTO>> GetAsync(TournamentsQueryFilter filter)
    {
        IEnumerable<Tournament> results = (await tournamentsRepository.GetAsync(
            filter.Limit,
            filter.Page - 1,
            filter,
            false
        )).ToList();

        var count = results.Count();
        var origPage = filter.Page;

        string? next = null;
        if (count == filter.Limit)
        {
            filter.Page = origPage + 1;
            next = urlHelperService.Action(new CreatedAtRouteValues
            {
                Action = nameof(TournamentsController.ListAsync),
                Controller = nameof(TournamentsController),
                RouteValues = filter.ToDictionary()
            });
        }

        string? previous = null;
        if (origPage > 1)
        {
            filter.Page = origPage - 1;
            previous = urlHelperService.Action(new CreatedAtRouteValues
            {
                Action = nameof(TournamentsController.ListAsync),
                Controller = nameof(TournamentsController),
                RouteValues = filter.ToDictionary()
            });
        }

        return new PagedResultDTO<TournamentDTO>
        {
            Next = next,
            Previous = previous,
            Count = count,
            Results = mapper.Map<IEnumerable<TournamentDTO>>(results).ToList()
        };
    }

    public async Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true) =>
        mapper.Map<TournamentDTO>(await tournamentsRepository.GetAsync(id, eagerLoad));

    public async Task<int> CountPlayedAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await tournamentsRepository.CountPlayedAsync(playerId, ruleset, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.MaxValue);

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
}
