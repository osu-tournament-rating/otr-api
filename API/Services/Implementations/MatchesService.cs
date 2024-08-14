using API.Controllers;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Repositories.Interfaces;
using NuGet.Protocol;

namespace API.Services.Implementations;

public class MatchesService(
    IMatchesRepository matchesRepository,
    ITournamentsRepository tournamentsRepository,
    IUrlHelperService urlHelperService,
    IMapper mapper
) : IMatchesService
{
    // TODO: Refactor to use enums for param "verificationSource"
    public async Task<IEnumerable<MatchCreatedResultDTO>?> CreateAsync(int tournamentId,
        int submitterId,
        IEnumerable<long> matchIds,
        bool verify)
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

    public async Task<PagedResultDTO<MatchDTO>> GetAsync(
        int limit,
        int page,
        MatchesFilterDTO filter
    )
    {
        IEnumerable<Match> result = await matchesRepository.GetAsync(
            limit,
            page - 1,
            ruleset: filter.Ruleset,
            name: filter.Name,
            dateMin: filter.DateMin,
            dateMax: filter.DateMax,
            verificationStatus: filter.VerificationStatus,
            rejectionReason: filter.RejectionReason,
            processingStatus: filter.ProcessingStatus,
            submittedBy: filter.SubmittedBy,
            verifiedBy: filter.VerifiedBy,
            querySortType: filter.Sort,
            sortDescending: filter.SortDescending
        );
        var count = result.Count();

        IDictionary<string, string> filterQuery = filter.ToDictionary();

        return new PagedResultDTO<MatchDTO>
        {
            Next = count == limit
                ? urlHelperService.Action(new CreatedAtRouteValues
                {
                    Controller = nameof(MatchesController),
                    Action = nameof(MatchesController.ListAsync),
                    RouteValues = filterQuery.Concat(new[]
                    {
                        new KeyValuePair<string, string>(nameof(limit), limit.ToString()),
                        new KeyValuePair<string, string>(nameof(page), (page + 1).ToString())
                    })
                })
                : null,
            Previous = page > 1
                ? urlHelperService.Action(new CreatedAtRouteValues
                {
                    Controller = nameof(MatchesController),
                    Action = nameof(MatchesController.ListAsync),
                    RouteValues = filterQuery.Concat(new[]
                    {
                        new KeyValuePair<string, string>(nameof(limit), limit.ToString()),
                        new KeyValuePair<string, string>(nameof(page), (page - 1).ToString())
                    })
                })
                : null,
            Count = count,
            Results = mapper.Map<IEnumerable<MatchDTO>>(result).ToList()
        };
    }

    public async Task<PagedResultDTO<MatchDTO>> GetAsync(
        int limit,
        int page,
        QueryFilterType filterType = QueryFilterType.Verified | QueryFilterType.ProcessingCompleted
    )
    {
        IEnumerable<Match> result = await matchesRepository.GetAsync(limit, page, filterType);
        var count = result.Count();

        return new PagedResultDTO<MatchDTO>
        {
            Next = count == limit
                ? urlHelperService.Action(new CreatedAtRouteValues
                {
                    Controller = nameof(MatchesController),
                    Action = nameof(MatchesController.ListAsync),
                    RouteValues = new { limit, page = page + 1 }
                })
                : null,
            Previous = page > 1
                ? urlHelperService.Action(new CreatedAtRouteValues
                {
                    Controller = nameof(MatchesController),
                    Action = nameof(MatchesController.ListAsync),
                    RouteValues = new { limit, page = page - 1 }
                })
                : null,
            Count = count,
            Results = mapper.Map<IEnumerable<MatchDTO>>(result).ToList()
        };
    }

    public async Task<MatchDTO?> GetAsync(
        int id,
        QueryFilterType filterType = QueryFilterType.Verified | QueryFilterType.ProcessingCompleted
    ) =>
        mapper.Map<MatchDTO?>(await matchesRepository.GetAsync(id, filterType));

    public async Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        int mode,
        DateTime start,
        DateTime end
    )
    {
        IEnumerable<Match> matches = await matchesRepository.GetPlayerMatchesAsync(osuPlayerId, mode, start, end);
        return mapper.Map<IEnumerable<MatchDTO>>(matches);
    }

    public async Task<IEnumerable<MatchSearchResultDTO>> SearchAsync(string name) =>
        mapper.Map<IEnumerable<MatchSearchResultDTO>>(await matchesRepository.SearchAsync(name));
}
