using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class MatchesService(
    IMatchesRepository matchesRepository,
    IPlayersRepository playersRepository,
    IMapper mapper
) : IMatchesService
{
    public async Task<IEnumerable<MatchDTO>> GetAsync(MatchRequestQueryDTO filter)
    {
        IEnumerable<Match> result = await matchesRepository.GetAsync(
            filter.Page,
            filter.PageSize,
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
            sortDescending: filter.Descending
        );

        return mapper.Map<IEnumerable<MatchDTO>>(result);
    }

    public async Task<MatchDTO?> GetAsync(int id, bool verified)
    {
        MatchDTO? match = mapper
            .Map<MatchDTO?>(await matchesRepository.GetFullAsync(id, verified));

        if (match is null)
        {
            return null;
        }

        ICollection<PlayerCompactDTO> players = await GetPlayerCompactsAsync(match);
        match.Players = players;

        return match;
    }

    public async Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        Ruleset ruleset,
        DateTime start,
        DateTime end
    )
    {
        var matches = (await matchesRepository.GetPlayerMatchesAsync(osuPlayerId, ruleset, start, end)).ToList();
        IEnumerable<MatchDTO>? matchDtos = mapper.Map<IEnumerable<MatchDTO>>(matches);

        foreach (MatchDTO dto in matchDtos)
        {
            dto.Players = await GetPlayerCompactsAsync(dto);
        }

        return matchDtos;
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

    public async Task<bool> ExistsAsync(int id) =>
        await matchesRepository.ExistsAsync(id);

    public async Task DeleteAsync(int id) =>
        await matchesRepository.DeleteAsync(id);

    private async Task<ICollection<PlayerCompactDTO>> GetPlayerCompactsAsync(MatchDTO match)
    {
        IEnumerable<int> playerIds = match.Games
            .Select(x => x.Scores.Select(y => y.PlayerId))
            .SelectMany(x => x)
            .Distinct();

        ICollection<PlayerCompactDTO>? players = mapper.Map<ICollection<PlayerCompactDTO>>(await playersRepository.GetAsync(playerIds));

        return players;
    }
}
