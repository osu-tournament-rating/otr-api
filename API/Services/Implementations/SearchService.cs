using API.DTOs;
using API.Entities;
using API.Handlers.Interfaces;
using API.Osu.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;

namespace API.Services.Implementations;

public class SearchService(
    ITournamentsRepository tournamentsRepository,
    IMatchesRepository matchesRepository,
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
            cacheHandler.Cache.GetObject<IEnumerable<TournamentSearchResultDTO>>(CacheUtils.TournamentSearchKey(tournamentName));

        if (result is not null)
        {
            return result;
        }

        result = (await tournamentsRepository.SearchAsync(tournamentName)).ToList();
        cacheHandler.SetTournamentSearchResult(result, tournamentName);

        return result;
    }

    private async Task<IEnumerable<MatchSearchResultDTO>> SearchMatchesByNameAsync(string matchName)
    {
        IEnumerable<MatchSearchResultDTO>? result =
            cacheHandler.Cache.GetObject<IEnumerable<MatchSearchResultDTO>>(CacheUtils.MatchSearchKey(matchName));

        if (result is not null)
        {
            return result;
        }

        result = (await matchesRepository.SearchAsync(matchName)).ToList();
        cacheHandler.SetMatchSearchResult(result, matchName);

        return result;
    }

    private async Task<IEnumerable<PlayerSearchResultDTO>> SearchPlayersByNameAsync(string username)
    {
        IEnumerable<PlayerSearchResultDTO>? result =
            cacheHandler.Cache.GetObject<IEnumerable<PlayerSearchResultDTO>>(CacheUtils.PlayerSearchKey(username));

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
        cacheHandler.SetPlayerSearchResult(result, username);

        return result;
    }
}
