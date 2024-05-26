using API.DTOs;
using API.Handlers.Interfaces;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using Database.Entities;
using Database.Enums;

namespace API.Services.Implementations;

public class SearchService(
    ITournamentsRepository tournamentsRepository,
    IMatchesService matchesService,
    IPlayerRepository playerRepository,
    ICacheHandler cacheHandler
    ) : ISearchService
{
    public async Task<SearchResponseCollectionDTO> SearchByNameAsync(string searchKey) =>
        new()
        {
            Tournaments = (await SearchTournamentsByNameAsync(searchKey)).ToList(),
            Matches = (await SearchMatchesByNameAsync(searchKey)).ToList(),
            Players = (await SearchPlayersByNameAsync(searchKey)).ToList()
        };

    private async Task<IEnumerable<TournamentSearchResultDTO>> SearchTournamentsByNameAsync(string tournamentName)
    {
        IEnumerable<TournamentSearchResultDTO>? result =
            await cacheHandler.Cache.GetObjectAsync<IEnumerable<TournamentSearchResultDTO>>(CacheUtils.TournamentSearchKey(tournamentName));

        if (result is not null)
        {
            return result;
        }

        result = (await tournamentsRepository.SearchAsync(tournamentName)).ToList();
        await cacheHandler.SetTournamentSearchResultAsync(result, tournamentName);

        return result;
    }

    private async Task<IEnumerable<MatchSearchResultDTO>> SearchMatchesByNameAsync(string matchName)
    {
        IEnumerable<MatchSearchResultDTO>? result =
            await cacheHandler.Cache.GetObjectAsync<IEnumerable<MatchSearchResultDTO>>(CacheUtils.MatchSearchKey(matchName));

        if (result is not null)
        {
            return result;
        }

        result = (await matchesService.SearchAsync(matchName)).ToList();
        await cacheHandler.SetMatchSearchResultAsync(result, matchName);

        return result;
    }

    private async Task<IEnumerable<PlayerSearchResultDTO>> SearchPlayersByNameAsync(string username)
    {
        IEnumerable<PlayerSearchResultDTO>? result =
            await cacheHandler.Cache.GetObjectAsync<IEnumerable<PlayerSearchResultDTO>>(CacheUtils.PlayerSearchKey(username));

        if (result is not null)
        {
            return result;
        }

        result = (await playerRepository.SearchAsync(username))
            .Select(player =>
            {
                BaseStats? stats = player.Ratings
                    .FirstOrDefault(r => r.Mode == (player.Ruleset ?? Ruleset.Standard));
                return new PlayerSearchResultDTO
                {
                    Id = player.Id,
                    OsuId = player.OsuId,
                    Rating = stats?.Rating,
                    GlobalRank = stats?.GlobalRank,
                    Username = player.Username,
                    Thumbnail = $"a.ppy.sh/{player.OsuId}"
                };
            })
            .ToList();
        await cacheHandler.SetPlayerSearchResultAsync(result, username);

        return result;
    }
}
