using API.DTOs;
using API.Handlers.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Database.Entities;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class SearchService(
    ITournamentsRepository tournamentsRepository,
    IMatchesService matchesService,
    IPlayersRepository playerRepository,
    ICacheHandler cacheHandler,
    IMapper mapper
) : ISearchService
{
    public async Task<SearchResponseCollectionDTO> SearchByNameAsync(string searchKey) =>
        new()
        {
            Tournaments = [.. await SearchTournamentsByNameAsync(searchKey)],
            Matches = [.. await SearchMatchesByNameAsync(searchKey)],
            Players = [.. await SearchPlayersByNameAsync(searchKey)]
        };

    private async Task<IEnumerable<TournamentSearchResultDTO>> SearchTournamentsByNameAsync(string tournamentName)
    {
        IList<TournamentSearchResultDTO>? result;

        try
        {
            result = await cacheHandler.Cache.GetObjectAsync<IList<TournamentSearchResultDTO>>(
                CacheUtils.TournamentSearchKey(tournamentName));

            if (result is not null)
            {
                return result;
            }
        }
        catch (Exception)
        {
            // Item failed to resolve from cache, continue with search
        }

        IList<Tournament> searchResult = await tournamentsRepository.SearchAsync(tournamentName);
        result = [.. searchResult.Select(t => mapper.Map<TournamentSearchResultDTO>(t))];

        await cacheHandler.SetTournamentSearchResultAsync(result, tournamentName);
        return result;
    }

    private async Task<IEnumerable<MatchSearchResultDTO>> SearchMatchesByNameAsync(string matchName)
    {
        IEnumerable<MatchSearchResultDTO>? result =
            await cacheHandler.Cache.GetObjectAsync<IEnumerable<MatchSearchResultDTO>>(
                CacheUtils.MatchSearchKey(matchName));

        if (result is not null)
        {
            return result;
        }

        result = [.. (await matchesService.SearchAsync(matchName))];
        await cacheHandler.SetMatchSearchResultAsync(result, matchName);

        return result;
    }

    private async Task<IEnumerable<PlayerSearchResultDTO>> SearchPlayersByNameAsync(string username)
    {
        var result =
            (await cacheHandler.Cache.GetObjectAsync<IEnumerable<PlayerSearchResultDTO>>(
                CacheUtils.PlayerSearchKey(username)))?.ToList();

        if (result != null && result.Count != 0)
        {
            return result;
        }

        result = [.. (await playerRepository.SearchAsync(username))
            .Select(player =>
            {
                PlayerRating? stats = player.Ratings
                    .FirstOrDefault(r => r.Ruleset == (player.User?.Settings.DefaultRuleset ?? player.DefaultRuleset));
                return new PlayerSearchResultDTO
                {
                    Id = player.Id,
                    OsuId = player.OsuId,
                    Rating = stats?.Rating,
                    Ruleset = stats?.Ruleset,
                    GlobalRank = stats?.GlobalRank,
                    Username = player.Username,
                    Thumbnail = $"a.ppy.sh/{player.OsuId}"
                };
            })];
        await cacheHandler.SetPlayerSearchResultAsync(result, username);

        return result;
    }
}
