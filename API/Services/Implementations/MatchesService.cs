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

public class MatchesService(
    IMatchesRepository matchesRepository,
    ITournamentsRepository tournamentsRepository,
    IUrlHelperService urlHelperService,
    IMapper mapper
) : IMatchesService
{
    public async Task<IEnumerable<MatchCreatedResultDTO>?> CreateAsync(
        int tournamentId,
        int submitterId,
        IEnumerable<long> matchIds,
        bool verify
    )
    {
        Tournament? tournament = await tournamentsRepository.GetAsync(tournamentId);
        if (tournament is null)
        {
            return null;
        }

        // Only create matches that dont already exist
        IEnumerable<long> enumerableMatchIds = matchIds.ToList();
        IEnumerable<long> existingMatchIds = (await matchesRepository.GetAsync(enumerableMatchIds))
            .Select(m => m.OsuId)
            .ToList();

        // Create matches directly on the tournament because we can't access their ids until after the entity is updated
        IEnumerable<long> createdMatchIds = enumerableMatchIds.Except(existingMatchIds).ToList();
        foreach (var matchId in createdMatchIds)
        {
            tournament.Matches.Add(new Match
            {
                OsuId = matchId,
                VerificationStatus = VerificationStatus.None,
                VerifiedByUserId = verify ? submitterId : null,
                SubmittedByUserId = submitterId
            });
        }

        await tournamentsRepository.UpdateAsync(tournament);
        IEnumerable<Match> createdMatches = tournament.Matches.Where(m => createdMatchIds.Contains(m.OsuId));

        return mapper.Map<IEnumerable<MatchCreatedResultDTO>>(createdMatches);
    }

    public async Task<PagedResultDTO<MatchDTO>> GetAsync(MatchesQueryFilter filter)
    {
        IEnumerable<Match> results = (await matchesRepository.GetAsync(
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
                Action = nameof(MatchesController.ListAsync),
                Controller = nameof(MatchesController),
                RouteValues = filter.ToDictionary()
            });
        }

        string? previous = null;
        if (origPage > 1)
        {
            filter.Page = origPage - 1;
            previous = urlHelperService.Action(new CreatedAtRouteValues
            {
                Action = nameof(MatchesController.ListAsync),
                Controller = nameof(MatchesController),
                RouteValues = filter.ToDictionary()
            });
        }

        return new PagedResultDTO<MatchDTO>
        {
            Next = next,
            Previous = previous,
            Count = count,
            Results = mapper.Map<IEnumerable<MatchDTO>>(results).ToList()
        };
    }

    public async Task<MatchDTO?> GetAsync(
        int id,
        QueryFilterType filterType = QueryFilterType.Verified | QueryFilterType.ProcessingCompleted
    ) =>
        mapper.Map<MatchDTO?>(await matchesRepository.GetAsync(id, filterType));

    public async Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        Ruleset ruleset,
        DateTime start,
        DateTime end
    )
    {
        IEnumerable<Match> matches = await matchesRepository.GetPlayerMatchesAsync(osuPlayerId, ruleset, start, end);
        return mapper.Map<IEnumerable<MatchDTO>>(matches);
    }

    public async Task<IEnumerable<MatchSearchResultDTO>> SearchAsync(string name) =>
        mapper.Map<IEnumerable<MatchSearchResultDTO>>(await matchesRepository.SearchAsync(name));
}
