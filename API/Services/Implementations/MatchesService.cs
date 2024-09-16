using API.Controllers;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class MatchesService(
    IMatchesRepository matchesRepository,
    IUrlHelperService urlHelperService,
    IMapper mapper
) : IMatchesService
{
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

    public async Task<MatchDTO?> GetAsync(int id) =>
        mapper.Map<MatchDTO?>(await matchesRepository.GetFullAsync(id));

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

    public async Task<MatchDTO?> UpdateAsync(int id, MatchDTO match)
    {
        Match? existing = await matchesRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.Name = match.Name;
        existing.StartTime = match.StartTime ?? existing.StartTime;
        existing.EndTime = match.EndTime ?? existing.EndTime;
        existing.VerificationStatus = match.VerificationStatus;
        existing.RejectionReason = match.RejectionReason;
        existing.ProcessingStatus = match.ProcessingStatus;

        await matchesRepository.UpdateAsync(existing);
        return mapper.Map<MatchDTO>(existing);
    }
}
