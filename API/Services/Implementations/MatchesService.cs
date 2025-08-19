using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class MatchesService(
    IMatchesRepository matchesRepository,
    IPlayersRepository playersRepository,
    IGameScoresRepository gameScoresRepository,
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
            submittedBy: filter.SubmittedBy,
            verifiedBy: filter.VerifiedBy,
            querySortType: filter.Sort,
            sortDescending: filter.Descending
        );

        return mapper.Map<IEnumerable<MatchDTO>>(result);
    }

    public async Task<MatchDTO?> GetAsync(int id)
    {
        MatchDTO? match = mapper.Map<MatchDTO?>(await matchesRepository.GetFullAsync(id));

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
        IEnumerable<MatchDTO> matchDtos = mapper.Map<IEnumerable<MatchDTO>>(matches).ToList();

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

        mapper.Map(match, existing);

        if (match.VerificationStatus == VerificationStatus.Rejected)
        {
            await matchesRepository.LoadGamesWithScoresAsync(existing);
            existing.RejectAllChildren();
        }

        await matchesRepository.UpdateAsync(existing);
        return mapper.Map<MatchDTO>(existing);
    }

    public async Task<bool> ExistsAsync(int id) =>
        await matchesRepository.ExistsAsync(id);

    public async Task<MatchDTO?> MergeAsync(int parentId, IEnumerable<int> matchIds) =>
        mapper.Map<MatchDTO?>(await matchesRepository.MergeAsync(parentId, matchIds));

    public async Task DeleteAsync(int id) =>
        await matchesRepository.DeleteAsync(id);

    public async Task<int> DeletePlayerScoresAsync(int matchId, int playerId)
    {
        if (!await matchesRepository.ExistsAsync(matchId))
        {
            return 0;
        }

        return await gameScoresRepository.DeleteByMatchAndPlayerAsync(matchId, playerId);
    }

    private async Task<ICollection<PlayerCompactDTO>> GetPlayerCompactsAsync(MatchDTO match)
    {
        IEnumerable<int> playerIds = match.Games
            .SelectMany(g => g.Scores.Select(s => s.PlayerId))
            .Distinct();

        ICollection<PlayerCompactDTO>? players =
            mapper.Map<ICollection<PlayerCompactDTO>>(await playersRepository.GetAsync(playerIds));

        return players;
    }
}
